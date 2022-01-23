using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using test.Contract;
using test.Model;

namespace test.Services
{
    public class ImportCsvFileService :IImportCsvFile
    {
        private readonly IEnumerable<IClothesRepository> _clothesRepositories;

        public ImportCsvFileService(IEnumerable<IClothesRepository> clothesRepositories)
        {
            _clothesRepositories = clothesRepositories;
        }

        public void Import(UploadedFileInfo csvFile)
        {
            using var reader = new StreamReader(csvFile.FullTempName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();
            List<Clothe> recordsToInsert = new List<Clothe>();
            while (csv.Read())
            {
                recordsToInsert.Add(csv.GetRecord<Clothe>());
                if (recordsToInsert.Count == 1000)
                {
                    foreach (var repo in _clothesRepositories)
                        repo.Insert(csvFile.FullTempName, recordsToInsert.ToArray());
                    recordsToInsert.Clear();
                }
            }
            if(recordsToInsert.Count>0)
                foreach (var repo in _clothesRepositories)
                    repo.Insert(csvFile.FullTempName, recordsToInsert.ToArray());
        }
        
    }
}
