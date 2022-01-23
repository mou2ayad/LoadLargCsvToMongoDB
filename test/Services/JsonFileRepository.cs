using System.IO;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using test.Config;
using test.Contract;

namespace test.Services
{
    public class JsonFileRepository : IFileRepository
    {
        private readonly JsonOutputConfig _config;
        private StreamWriter _streamWriter;
        private string _prefix;
        public JsonFileRepository(IOptions<JsonOutputConfig> config)
        {
            _config = config.Value;
        }

        public void Write(object ob)
        {
            _streamWriter.Write(_prefix);
            _streamWriter.WriteLine(JsonConvert.SerializeObject(ob,Formatting.Indented));
            _prefix=",";
        }

        public void CloseFile()
        {
            _streamWriter.WriteLine("]");
            Dispose();
        }

        public void Dispose() => _streamWriter.DisposeAsync();


        public void OpenFile(string fromFile)
        {
            var filePath = Path.Combine(_config.JsonOutputDirectory, fromFile + ".json");
            if (!_config.OverwriteIfExists && File.Exists(filePath))
                throw new IOException($"File with name {filePath} already exist");

            _streamWriter = File.CreateText(filePath);
            _streamWriter.WriteLine("[");
            _prefix = "";

        }

    }
}
