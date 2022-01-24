using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CSVUploaderAPI.Model;
using MongoDB.Driver;

namespace CSVUploaderApi.Test.Utils
{
    public static class ClothesStorage
    {
        public static void Delete(string key) => GetStoredObjects().DeleteOne(p => p.Clothe.Key == key);

        public static ClotheDoa Read(string key) => GetStoredObjects().Find(c => c.Clothe.Key == key).FirstOrDefault();
        public static List<ClotheDoa> Get(Expression<Func<ClotheDoa, bool>> filter)=> GetStoredObjects().Find(filter).ToList();
        private static IMongoCollection<ClotheDoa> GetStoredObjects()
        {
            var db = Mongo2Test.GetMongoDbClient().GetDatabase("ClothesDB");
            return db.GetCollection<ClotheDoa>("Clothes");
        }
    }
}
