using System.Threading.Tasks;
using CSVUploaderAPI.Bus;
using CSVUploaderAPI.Contract;
using SlimMessageBus;

namespace CSVUploaderAPI.Services
{
    public class ImportToMongoDbHandler : IConsumer<CsvRecordParsedEvent>
    {
        private readonly IDbRepository _repository;

        public ImportToMongoDbHandler(IDbRepository repository) =>
            _repository = repository;

        public async Task OnHandle(CsvRecordParsedEvent message, string path) =>
            await _repository.Insert(message.FromFile, message.Record);
    }
}
