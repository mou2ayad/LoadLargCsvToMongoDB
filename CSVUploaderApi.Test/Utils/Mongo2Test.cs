using Mongo2Go;
using MongoDB.Driver;

namespace CSVUploaderApi.Test.Utils
{
    public static class Mongo2Test
    {
        private static MongoDbRunner _mongoDbRunner;
        private static MongoClient _mongoDbClient;

        public static MongoClient GetMongoDbClient() => _mongoDbClient ??= CreateClient();

        private static MongoClient CreateClient()
        {
            _mongoDbRunner = MongoDbRunner.Start();

            return new MongoClient(_mongoDbRunner.ConnectionString);
        }

        public static void Close()
        {
            _mongoDbClient = null;
            _mongoDbRunner?.Dispose();
        }
    }
   
}
