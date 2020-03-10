using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Framework.DB.MongoDB.Repository
{
//Wrapper for mongo db and client
    public class MongoDbContext : IMongoDbContext
    {
        protected readonly IMongoDatabase Db;
        protected readonly ILogger<MongoDbContext> Logger;
        private readonly IMongoClient _client;
        private readonly ICollectionNameProvider _collectionNameProvider;

        public MongoDbContext(string connectionString, ILogger<MongoDbContext> logger,  ICollectionNameProvider collectionNameProvider)
        {
            var connection = new MongoUrlBuilder(connectionString);
            _client = new MongoClient(connectionString);
            Db = _client.GetDatabase(connection.DatabaseName);
            Logger = logger;
            _collectionNameProvider = collectionNameProvider;
        }

        public IMongoDatabase Database => Db;
        public IMongoClient Client => _client;

        public IMongoCollection<T> GetCollection<T>()
        {
            var collectionName = _collectionNameProvider.GetCollectionName(typeof(T));
            return Db.GetCollection<T>(collectionName);
        }

        public IMongoQueryable<T> GetQueryableCollection<T>()
        {
            return GetCollection<T>().AsQueryable<T>();
        }

        public async Task CreateCollectionsAsync(IEnumerable<string> collectionNames)
        {
            foreach (var collection in collectionNames)
            {
                await Database.CreateCollectionAsync(collection);
            }
        }

    }
}