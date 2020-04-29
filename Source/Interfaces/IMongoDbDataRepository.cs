using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Framework.DB.MongoDB.Repository
{
    public interface IMongoDbDataRepository : IDataRepository
    {
        IClientSessionHandle StartSession();

        Task Initialize(IEnumerable<string> collectionNames);

        Task UpdateOneAsync<T, TKey>(T entityToModify, UpdateDefinition<T> update)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>; 
        
        Task UpdateOneAsync<T, TKey>(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle session)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>;
        Task UpdateOneAsync<T>(T entityToModify, UpdateDefinition<T> update)
            where T : IBaseEntity<ObjectId>;
        
        Task UpdateOneAsync<T>(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle sessionHandle)
            where T : IBaseEntity<ObjectId>;

        Task UpdateOneAsync<T, TKey, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>;

        Task UpdateOneAsync<T, TKey, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>;

        Task UpdateOneAsync<T, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value)
            where T : IBaseEntity<ObjectId>;
        
        Task UpdateOneAsync<T, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session)
            where T : IBaseEntity<ObjectId>;

        Task UpdateManyAsync<T, TKey>(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
           where T : IBaseEntity<TKey>
           where TKey : IEquatable<TKey>;

        Task UpdateManyAsync<T, TKey>(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, IClientSessionHandle session)
           where T : IBaseEntity<TKey>
           where TKey : IEquatable<TKey>;

        Task UpdateManyAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
           where T : IBaseEntity<ObjectId>;

        Task UpdateManyAsync<T>(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, IClientSessionHandle session)
            where T : IBaseEntity<ObjectId>;

        Task UpdateManyAsync<T, TKey, TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value)
          where T : IBaseEntity<TKey>
          where TKey : IEquatable<TKey>;

        Task UpdateManyAsync<T, TKey, TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>;

        Task UpdateManyAsync<T, TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value)
            where T : IBaseEntity<ObjectId>;

        Task UpdateManyAsync<T, TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session)
            where T : IBaseEntity<ObjectId>;
    }
}