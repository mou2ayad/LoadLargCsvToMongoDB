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
        
        public async Task Import(UploadedFileInfo csvFile)
        {
            using var reader = new StreamReader(csvFile.FullTempName);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            await csv.ReadAsync();
            csv.ReadHeader();
            await _jsonRepository.OpenFile(csvFile.OriginalName);
            while (await csv.ReadAsync())
            {
                var @event = CsvRecordParsedEvent.From(csv.GetRecord<Clothe>(), csvFile.OriginalName);
                Task dispatchTask= _dispatcher.Dispatch(@event);
                Task writeJsonTask= _jsonRepository.Write(@event.Record);
                await Task.WhenAll(dispatchTask, writeJsonTask);
            }
            await _jsonRepository.CloseFile();
        }

        public async Task OnHandle(FileUploadedEvent message, string path)
        {
            await Import(message.UploadedFile);
        }

    }
}
