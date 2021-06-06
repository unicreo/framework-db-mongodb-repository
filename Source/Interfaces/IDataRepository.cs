using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;

namespace Framework.DB.MongoDB.Repository
{
    /// <summary>
    /// General db independent repository interface
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <typeparam name="TKey">Type of Id field</typeparam>
    public interface IDataRepository<T, TKey>
        where T : IBaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        Task<T> GetAsync(string id);

        Task<IEnumerable<T>> GetListAsync(int? skip = null, int? take = null, Expression<Func<T, bool>> filter = null);

        Task AddAsync(T entity);

        Task AddListAsync(IEnumerable<T> entities);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);

        Task DeleteManyAsync(Expression<Func<T, bool>> filter);

        Task<long> CountAsync(Expression<Func<T, bool>> filter);
    }
}