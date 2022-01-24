using System;
using System.Linq;
using System.Threading.Tasks;
using CSVUploaderAPI.Contract;
using CSVUploaderAPI.Model;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CSVUploaderAPI.Services
{
    public class MongoDbClothesRepository :IDbRepository
    {
        private const string CollectionName = "Clothes";
        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbClothesRepository> _logger;
        private IMongoCollection<ClotheDoa> Collection => _database.GetCollection<ClotheDoa>(CollectionName);

        public MongoDbClothesRepository(IMongoClient client, ILogger<MongoDbClothesRepository> logger)
        {
            _logger = logger;
            _database = client.GetDatabase("ClothesDB");
        }

        private async Task InsertMany(string fromFile, params Clothe[] clothes)
        {
            try
            {
                await Collection.InsertManyAsync(clothes.Select(e=>ClotheDoa.From(e, fromFile)));
                _logger.LogInformation($"{clothes.Length} clothes are created");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{clothes.Length} clothe add failed.");
                throw;
            }
        }

        private async Task InsertOne(string fromFile, Clothe clothe)
        {
            try
            {
                await Collection.InsertOneAsync(ClotheDoa.From(clothe, fromFile));
                _logger.LogInformation("clothe {@clothe} is created", clothe.Key);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "clothe add {@clothe} failed.", clothe.Key);
                throw;
            }
        }

        public Task Insert(string fromFile, params Clothe[] clothes)
            => clothes.Length > 1 ?
            InsertMany(fromFile, clothes) : 
            InsertOne(fromFile, clothes.FirstOrDefault());

    }
}
