using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using IClientSessionHandle = MongoDB.Driver.IClientSessionHandle;

namespace Framework.DB.MongoDB.Repository
{
    public class MongoDbDataRepository: IMongoDbDataRepository
    {
        protected readonly IMongoDbContext DbContext;
        protected readonly ILogger<MongoDbDataRepository> Logger;

        public MongoDbDataRepository(IMongoDbContext dbContext, ILogger<MongoDbDataRepository> logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        public IClientSessionHandle StartSession() => DbContext.Client.StartSession();

        /// <summary>
        /// Get document of type T from db by Id for reading
        /// </summary>
        /// <param name="id"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetDocumentAsync<T>(string id) where T : IBaseEntity<ObjectId>
        {
            var cursor = await DbContext.GetCollection<T>().FindAsync(_ => _.Id == ObjectId.Parse(id));
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// Get list of objects of type T from db only for reading
        /// </summary>
        /// <param name="filter">Filter a sequense of values based on a predicate</param>
        public async Task<T> GetDocumentAsync<T>(Expression<Func<T, bool>> filter = null) where T : class
        {
            if (filter == null) filter = (_ => true);
            var cursor = await DbContext.GetCollection<T>().FindAsync(filter);
            var result = await cursor.FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// Get list of objects of type T from db only for reading
        /// </summary>
        /// <param name="take"></param>
        /// <param name="filter">Filter a sequense of values based on a predicate</param>
        /// <param name="skip"></param>
        public async Task<IEnumerable<T>> GetListAsync<T>(
            int? skip = null,
            int? take = null,
            Expression<Func<T, bool>> filter = null) where T : class
        {
            if (filter == null) filter = (_ => true);
            var query = DbContext.GetCollection<T>().Find(filter);
            if (skip != null && take != null)
            {
                query.Skip(skip).Limit(take);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Get list of objects of type T from db only for reading
        /// </summary>
        /// <param name="filter">Filter a sequense of values based on a predicate</param>
        public async Task<IEnumerable<T>> GetListAsync<T>(
            Expression<Func<T, bool>> filter) where T : class
        {
            if (filter == null) filter = (_ => true);
            var query = DbContext.GetCollection<T>().Find(filter);
            return await query.ToListAsync();
        }


        /// <summary>
        /// Insert single entity in db asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task AddAsync<T>(T entity) where T : class =>
            await DbContext.GetCollection<T>().InsertOneAsync(entity);
        
        
        /// <summary>
        /// Insert single entity in db asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task AddAsync<T>(T entity, IClientSessionHandle session) where T : class =>
            await DbContext.GetCollection<T>().InsertOneAsync(session, entity);


        /// <summary>
        /// Insert list of entities in db asynchronously
        /// </summary>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        /// <param name="entities"></param>
        public async Task AddListAsync<T>(IEnumerable<T> entities) where T : class =>
            await DbContext.GetCollection<T>().InsertManyAsync(entities);
        
        /// <summary>
        /// Insert list of entities in db asynchronously
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task AddListAsync<T>(IEnumerable<T> entities, IClientSessionHandle session) where T : class =>
            await DbContext.GetCollection<T>().InsertManyAsync(session, entities);
 
        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task UpdateAsync<T, TKey>(T entity)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);
        
        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task UpdateAsync<T, TKey>(T entity, IClientSessionHandle session)
         where T : IBaseEntity<TKey>
         where TKey : IEquatable<TKey> =>
             await DbContext.GetCollection<T>().ReplaceOneAsync(session,x => x.Id.Equals(entity.Id), entity);


        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task UpdateAsync<T>(T entity)
          where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);
        
        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task UpdateAsync<T>(T entity, IClientSessionHandle session)
          where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().ReplaceOneAsync(session,x => x.Id.Equals(entity.Id), entity);


        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a Document</typeparam>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="update">The update definition for the document</param>
        /// <returns></returns>
        public async Task UpdateOneAsync<T, TKey>(T entityToModify, UpdateDefinition<T> update)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), update);
        
        
        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb with session.
        /// </summary>
        /// <param name="entityToModify"></param>
        /// <param name="update"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task UpdateOneAsync<T, TKey>(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle session)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(session,x => x.Id.Equals(entityToModify.Id), update);
            

        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="update">The update definition for the document</param>
        /// <returns></returns>
        public async Task UpdateOneAsync<T>(T entityToModify, UpdateDefinition<T> update)
          where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), update);
        
        
        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb with session
        /// </summary>
        /// <param name="entityToModify"></param>
        /// <param name="update"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task UpdateOneAsync<T>(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle session)
          where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(session,x => x.Id.Equals(entityToModify.Id), update);
            

        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a Document</typeparam>
        /// <typeparam name="TField">The updated type of field</typeparam>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="field">The expression of updated field</param>
        /// <param name="value">The value of updated field</param>
        /// <returns></returns>
        public async Task UpdateOneAsync<T, TKey, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value)
          where T : IBaseEntity<TKey>
          where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));

        public async Task UpdateOneAsync<T, TKey, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session) where T : IBaseEntity<TKey> where TKey : IEquatable<TKey> =>
            await DbContext.GetCollection<T>().UpdateOneAsync(session,x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));


        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <typeparam name="TField">The updated type of field</typeparam>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="field">The expression of updated field</param>
        /// <param name="value">The value of updated field</param>
        /// <returns></returns>
        public async Task UpdateOneAsync<T, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value)
         where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));
        
        
        
        /// <summary>
        /// Updates the property field with the given value update a property field in entities with session
        /// </summary>
        /// <param name="entityToModify"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="session"></param>
        /// <typeparam name="T">Collection type</typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <returns></returns>
        public async Task UpdateOneAsync<T, TField>(T entityToModify, Expression<Func<T, TField>> field, TField value, IClientSessionHandle session)
         where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().UpdateOneAsync(session,x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));
        

        /// <summary>
        /// Delete entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task DeleteAsync<T, TKey>(T entity)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().DeleteOneAsync(x => x.Id.Equals(entity.Id));
        
        
        
        /// <summary>
        /// Delete entity asynchronously with session
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T">Collection type</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns></returns>
        public async Task DeleteAsync<T, TKey>(T entity, IClientSessionHandle session)
            where T : IBaseEntity<TKey>
            where TKey : IEquatable<TKey> =>
                await DbContext.GetCollection<T>().DeleteOneAsync(session,x => x.Id.Equals(entity.Id));
            

        /// <summary>
        /// Delete entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task DeleteAsync<T>(T entity)
            where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().DeleteOneAsync(x => x.Id.Equals(entity.Id));
        
        
        /// <summary>
        /// Delete entity asynchronously with session
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <returns></returns>
        public async Task DeleteAsync<T>(T entity, IClientSessionHandle session)
            where T : IBaseEntity<ObjectId> =>
                await DbContext.GetCollection<T>().DeleteOneAsync(session,x => x.Id.Equals(entity.Id));

        public async Task Initialize(IEnumerable<string> collectionNames)
        {
            var availableTables = DbContext.Database.ListCollectionNames().ToList();
            var absentTables = (collectionNames).Except(availableTables);
            await DbContext.CreateCollectionsAsync(absentTables);
        }

    }
    
}