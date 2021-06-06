using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;
using MongoDB.Driver;

namespace Framework.DB.MongoDB.Repository
{
    /// <summary>
    /// Mongo DB concrete repository interface, with mongo db related extensions and string as a Id Key 
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>    
    public interface IMongoDbDataRepository<T> : IDataRepository<T, string>
        where T : IBaseEntity<string>
    {
        IClientSessionHandle StartSession();

        Task<T> GetAsync(string id, ProjectionDefinition<T> projection = null);

        Task<T> GetAsync(Expression<Func<T, bool>> filter, ProjectionDefinition<T> projection = null);

        Task<IEnumerable<T>> GetListAsync(int? skip = null, int? take = null, Expression<Func<T, bool>> filter = null, ProjectionDefinition<T> projection = null);

        Task AddAsync(T entity, IClientSessionHandle session);

        Task UpdateOneAsync(T entityToModify, UpdateDefinition<T> update);

        Task UpdateOneAsync(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle session);

        Task UpdateOneAsync<TField>(T entityToModify, Expression<Func<T, TField>> field, TField value);

        Task UpdateOneAsync<TField>(T entityToModify, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session);

        Task UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update);

        Task UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update, IClientSessionHandle session);

        Task UpdateManyAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value);

        Task UpdateManyAsync<TField>(Expression<Func<T, bool>> filter, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session);
    }
}