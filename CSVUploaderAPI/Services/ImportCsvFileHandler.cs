using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CSVUploaderAPI.Bus;
using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Model;
using SlimMessageBus;

namespace CSVUploaderAPI.Services
{
    public class ImportCsvFileService :IConsumer<FileUploadedEvent>
    {
        private readonly IEventDispatcher<CsvRecordParsedEvent> _dispatcher;
        private readonly IFileRepository _jsonRepository;
        public ImportCsvFileService(IFileRepository repository, IEventDispatcher<CsvRecordParsedEvent> dispatcher)
        {
            _jsonRepository = repository;
            _dispatcher = dispatcher;
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
                _dispatcher.Dispatch(@event);
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
