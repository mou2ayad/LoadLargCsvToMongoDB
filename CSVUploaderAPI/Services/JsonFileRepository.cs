using System.IO;
using System.Threading.Tasks;
using CSVUploaderAPI.Config;
using CSVUploaderAPI.Contract;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CSVUploaderAPI.Services
{
    public class JsonFileRepository : IFileRepository
    {
        private readonly JsonOutputConfig _config;
        private StreamWriter _streamWriter;
        private string _prefix;
        public JsonFileRepository(IOptions<JsonOutputConfig> config) => 
            _config = config.Value;

        public async Task Write(object ob)
        {
            await _streamWriter.WriteAsync(_prefix);
            await _streamWriter.WriteLineAsync(JsonConvert.SerializeObject(ob,Formatting.Indented));
            _prefix=",";
        }

        public async Task CloseFile()
        {
            await _streamWriter.WriteLineAsync("]");
            Dispose();
        }

        public void Dispose() => _streamWriter.DisposeAsync();

        public async Task OpenFile(string fromFile)
        {
            var filePath = Path.Combine(_config.JsonOutputDirectory, fromFile + ".json");
            if (!_config.OverwriteIfExists && File.Exists(filePath))
                throw new IOException($"File with name {filePath} already exist");

            _streamWriter = File.CreateText(filePath);
            await _streamWriter.WriteLineAsync("[");
            _prefix = "";
        }
    }
}