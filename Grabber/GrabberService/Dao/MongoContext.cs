using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrabberService.Dao.Interfaces;
using GrabberService.Properties;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GrabberService.Dao
{
    public class MongoContext : IMongoContext
    {
        private readonly MongoClient client;
        private readonly IMongoDatabase database;
        private readonly string collectionName;

        public MongoContext(AppSettings appSettings)
        {

            client = new MongoClient(appSettings.MongoDbConnectionString);
            database = client.GetDatabase(appSettings.MongoDbName);
            collectionName = appSettings.WeatherCollectionName;
        }

        public async Task Add<T>(T input)
        {
            await Execute<BsonDocument>(query => query.InsertOneAsync(input.ToBsonDocument()));
        }
        
        public async Task AddMany<T>(IEnumerable<T> input)
        {
            await Execute<BsonDocument>(query => query.InsertManyAsync(input.Select(i => i.ToBsonDocument()))).ConfigureAwait(false);
        }
        
        private async Task Execute<T>(Func<IMongoCollection<T>, Task> query)
        {
            IMongoCollection<T> collection = database.GetCollection<T>(collectionName);
            await query(collection).ConfigureAwait(false);
        }
    }
}