using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Core.Events;
using System.Linq;
using MongoDB.Bson;

namespace Framework.DB.MongoDB.Repository
{
    //Wrapper for mongo db and client
    public class MongoDbContext : IMongoDbContext
    {
        protected readonly IMongoDatabase Db;
        protected readonly ILogger<MongoDbContext> Logger;
        private readonly IMongoClient _client;
        private readonly ICollectionNameProvider _collectionNameProvider;

        public MongoDbContext(string connectionString,
            ILogger<MongoDbContext> logger,
            ICollectionNameProvider collectionNameProvider)
        {
            var connection = new MongoUrlBuilder(connectionString);

            // check db is set in the connection string and not admin db
            if (string.IsNullOrEmpty(connection.DatabaseName) || connection.DatabaseName.ToLower() == "admin")
            {
                throw new ArgumentException("DB must be set in the connection string and not be admin.");
            }

#if DEBUG

            _client = new MongoClient(connectionString);
            var settigs = MongoClientSettings.FromConnectionString(connectionString);
            settigs.ClusterConfigurator = cb => {
                cb.Subscribe<CommandStartedEvent>(e => {
                    logger.LogInformation($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            _client = new MongoClient(settigs);
#else
            _client = new MongoClient(connectionString);
#endif

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

        /// <summary>
        /// Initialize database collections with given collection names
        /// </summary>
        /// <param name="collectionNames"></param>
        /// <returns></returns>
        public async Task Initialize(IEnumerable<string> collectionNames)
        {
            var availableTables = Db.ListCollectionNames().ToList();
            var absentTables = (collectionNames).Except(availableTables);
            await CreateCollectionsAsync(absentTables);
        }

        private async Task CreateCollectionsAsync(IEnumerable<string> collectionNames)
        {
            foreach (var collection in collectionNames)
            {
                await Database.CreateCollectionAsync(collection);
            }
        }
    }
}