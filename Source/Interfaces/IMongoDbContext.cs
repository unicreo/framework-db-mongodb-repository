using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Framework.DB.MongoDB.Repository
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>();
        IMongoQueryable<T> GetQueryableCollection<T>();
        IMongoDatabase Database { get; }
        IMongoClient Client { get; }
        Task Initialize(IEnumerable<string> collectionNames);
    }
}