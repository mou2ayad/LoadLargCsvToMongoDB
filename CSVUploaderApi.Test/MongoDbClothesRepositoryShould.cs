using System;
using System.Linq;
using CSVUploaderAPI.Model;
using CSVUploaderAPI.Services;
using CSVUploaderApi.Test.Utils;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;

namespace CSVUploaderApi.Test
{
    public class MongoDbClothesRepositoryShould : UsingMongoDb2Test
    {
        [Test]
        public async Task Save_clothe_object_into_Db_with_correct_log()
        {
            string key = Guid.NewGuid().ToString();
            var logger = CreateLogger();
            var sut = Sut(logger);

            await sut.Insert("AnyFileName", new Clothe {Key = key});
            var result= ClothesStorage.Read(key);

            result.FromFile.Should().Be("AnyFileName");
            result.Clothe.Key.Should().Be(key);

            logger.IsLogAdded.Should().BeTrue();
            logger.AddedLogLevel.Should().Be(LogLevel.Information);
            logger.LoggedMessage.Should().Be($"clothe {key} is created");

            ClothesStorage.Delete(key);
        }


        [Test]
        public async Task Save_bulk_of_clothes_objects_into_Db_with_correct_log()
        {
            string key = Guid.NewGuid().ToString();

            string[] keys = {  key + "_1", key + "_2" }; 
            string fromFile = "AnyFileName";
            var logger = CreateLogger();
            var sut = Sut(logger);

            await sut.Insert("AnyFileName", keys.Select(e=>new Clothe(){Key = e}).ToArray());
            var result = ClothesStorage.Get(e => e.FromFile == fromFile);

            result.Select(e=>e.Clothe.Key).Except(keys).Count().Should().Be(0);

            logger.IsLogAdded.Should().BeTrue();
            logger.AddedLogLevel.Should().Be(LogLevel.Information);
            logger.LoggedMessage.Should().Be("2 clothes are created");

            ClothesStorage.Delete(key + "_1");
            ClothesStorage.Delete(key + "_2");

        }

        private static MongoDbClothesRepository Sut(ILogger<MongoDbClothesRepository> logger) =>
            new(Mongo2Test.GetMongoDbClient(), logger);

        private static FakeLogger<MongoDbClothesRepository> CreateLogger() => FakeLogger<MongoDbClothesRepository>.Create();

    }
}