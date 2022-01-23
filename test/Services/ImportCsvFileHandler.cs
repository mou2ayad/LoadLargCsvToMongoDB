using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using SlimMessageBus;
using test.Bus;
using test.Contract;
using test.Model;

namespace test.Services
{
    public class ImportCsvFileService :IConsumer<FileUploadedEvent>
    {
        private readonly IMessageBus _bus;
        private readonly IFileRepository _jsonRepository;
        public ImportCsvFileService(IMessageBus bus, IFileRepository repository )
        {
            _bus = bus;
            _jsonRepository = repository;
        }
        
        public void Import(UploadedFileInfo csvFile)
        {
            using var reader = new StreamReader(csvFile.FullTempName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();
            _jsonRepository.OpenFile(csvFile.OriginalName);
            while (csv.Read())
            {
                var @event = CsvRecordParsedEvent.From(csv.GetRecord<Clothe>(), csvFile.OriginalName);
                _bus.Publish(@event);
                _jsonRepository.Write(@event.Record);
            }
            _jsonRepository.CloseFile();
        }

        public Task OnHandle(FileUploadedEvent message, string path)
        {
            Import(message.UploadedFile);
            return Task.CompletedTask;
        }

    }
}
