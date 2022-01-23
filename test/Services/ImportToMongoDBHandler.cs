using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlimMessageBus;
using test.Bus;
using test.Contract;

namespace test.Services
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
