using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;
using MongoDB.Bson;

namespace Framework.DB.MongoDB.Repository
{
    public interface IDataRepository
    {
        Task<T> GetDocumentAsync<T>(Expression<Func<T, bool>> filter = null) where T : class;
        Task<T> GetDocumentAsync<T>(string id) where T : IBaseEntity<ObjectId>;
        Task<IEnumerable<T>> GetListAsync<T>(int? skip = null, int? take = null, Expression<Func<T, bool>> filter = null) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>(Expression<Func<T, bool>> filter) where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task AddListAsync<T>(IEnumerable<T> entities) where T : class;
        Task UpdateAsync<T, TKey>(T entity)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>; 
        
        Task UpdateAsync<T>(T entity)
            where T : IBaseEntity<ObjectId>;
        
        Task DeleteAsync<T, TKey>(T entity)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey>;

        Task DeleteAsync<T>(T entity)
            where T : IBaseEntity<ObjectId>;

    }
}