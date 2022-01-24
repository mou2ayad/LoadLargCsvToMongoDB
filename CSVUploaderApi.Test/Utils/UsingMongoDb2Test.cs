using NUnit.Framework;

namespace CSVUploaderApi.Test.Utils
{
    public class UsingMongoDb2Test
    {
        [OneTimeSetUp]
        public void Init() => Mongo2Test.GetMongoDbClient();

        [OneTimeTearDown]
        public void Cleanup() => Mongo2Test.Close();
    }
}
