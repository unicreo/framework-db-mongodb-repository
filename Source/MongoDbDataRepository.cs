﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Framework.DB.MongoDB.Repository.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Framework.DB.MongoDB.Repository
{
    /// <summary>
    /// Mongo DB concrete repository, with string as a Id Key
    /// <typeparam name="T">Entity type</typeparam>
    /// </summary>
    public class MongoDbDataRepository<T> : IMongoDbDataRepository<T>
             where T : IBaseEntity<string>
    {
        protected readonly IMongoDbContext DbContext;
        protected readonly ILogger<MongoDbDataRepository<T>> Logger;

        public MongoDbDataRepository(IMongoDbContext dbContext, ILogger<MongoDbDataRepository<T>> logger)
        {
            DbContext = dbContext;
            Logger = logger;
        }

        public IClientSessionHandle StartSession() => DbContext.Client.StartSession();

        public async Task<T> GetAsync(string id)
            => await GetAsync(id, null);


        /// <summary>
        /// Get document of type T from db by Id for reading
        /// </summary>
        /// <param name="id"></param>
        /// <param name="projection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> GetAsync(string id, ProjectionDefinition<T> projection = null)
        {
            var query = DbContext.GetCollection<T>().Find(_ => _.Id.Equals(id));

            if (projection != null)
            {
                query.Options.Projection = projection;
            }

            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
            => await GetAsync(filter, null);

        /// <summary>
        /// Get list of objects of type T from db only for reading
        /// </summary>
        /// <param name="filter">Filter a sequense of values based on a predicate</param>
        /// <param name="projection"></param>
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter, ProjectionDefinition<T> projection = null)
        {
            var query = DbContext.GetCollection<T>().Find(filter);

            if (projection != null)
            {
                query.Options.Projection = projection;
            }

            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        public async Task<IEnumerable<T>> GetListAsync(
           int? skip = null,
           int? take = null,
           Expression<Func<T, bool>> filter = null) =>
                await GetListAsync(skip, take, filter, null);

        /// <summary>
        /// Get list of objects of type T from db only for reading
        /// </summary>
        /// <param name="take"></param>
        /// <param name="filter">Filter a sequense of values based on a predicate</param>
        /// <param name="skip"></param>
        /// <param name="projection"></param>
        public async Task<IEnumerable<T>> GetListAsync(
            int? skip = null,
            int? take = null,
            Expression<Func<T, bool>> filter = null,
            ProjectionDefinition<T> projection = null)
        {
            if (filter == null)
            {
                filter = (_ => true);
            }
            var query = DbContext.GetCollection<T>().Find(filter);
            if (skip != null && take != null)
            {
                query.Skip(skip).Limit(take);
            }
            if (projection != null)
            {
                query.Options.Projection = projection;
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Insert single entity in db asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task AddAsync(T entity) =>
            await DbContext.GetCollection<T>().InsertOneAsync(entity);


        /// <summary>
        /// Insert single entity in db asynchronously with session
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task AddAsync(T entity, IClientSessionHandle session) =>
            await DbContext.GetCollection<T>().InsertOneAsync(session, entity);


        /// <summary>
        /// Insert list of entities in db asynchronously
        /// </summary>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        /// <param name="entities"></param>
        public async Task AddListAsync(IEnumerable<T> entities) =>
            await DbContext.GetCollection<T>().InsertManyAsync(entities);


        /// <summary>
        /// Insert list of entities in db asynchronously with session
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task AddListAsync(IEnumerable<T> entities, IClientSessionHandle session) =>
            await DbContext.GetCollection<T>().InsertManyAsync(session, entities);


        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task UpdateAsync(T entity) =>
                await DbContext.GetCollection<T>().ReplaceOneAsync(x => x.Id.Equals(entity.Id), entity);

        /// <summary>
        /// Update entity asynchronously
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task UpdateAsync(T entity, IClientSessionHandle session) =>
                await DbContext.GetCollection<T>().ReplaceOneAsync(session, x => x.Id.Equals(entity.Id), entity);

        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="update">The update definition for the document</param>
        /// <returns></returns>
        public async Task UpdateOneAsync(T entityToModify, UpdateDefinition<T> update) =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), update);


        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb with session
        /// </summary>
        /// <param name="entityToModify"></param>
        /// <param name="update"></param>
        /// <param name="session"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task UpdateOneAsync(T entityToModify, UpdateDefinition<T> update, IClientSessionHandle session) =>
                await DbContext.GetCollection<T>().UpdateOneAsync(session, x => x.Id.Equals(entityToModify.Id), update);


        /// <summary>
        /// Takes a document you want to modify and updates the field in entity with the given value.
        /// </summary>
        /// <typeparam name="T">The type representing a Document</typeparam>
        /// <typeparam name="TField">The updated type of field</typeparam>
        /// <param name="entityToModify">Db entity of type T</param>
        /// <param name="field">The expression of updated field</param>
        /// <param name="value">The value of updated field</param>
        /// <returns></returns>
        public async Task UpdateOneAsync<TField>(T entityToModify,
            Expression<Func<T, TField>> field,
            TField value) =>
                await DbContext.GetCollection<T>().UpdateOneAsync(x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));


        /// <summary>
        /// Takes a document you want to modify and updates the field in entity with the given value with MongoDb session.
        /// </summary>
        /// <param name="entityToModify"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="session"></param>
        /// <typeparam name="T">Collection type</typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <returns></returns>
        public async Task UpdateOneAsync<TField>(T entityToModify,
            Expression<Func<T, TField>> field,
            TField value,
            IClientSessionHandle session) =>
                await DbContext.GetCollection<T>().UpdateOneAsync(session, x => x.Id.Equals(entityToModify.Id), Builders<T>.Update.Set(field, value));


        /// <summary>
        /// Takes documents you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(Expression<Func<T, bool>> filter,
            UpdateDefinition<T> update) =>
                await DbContext.GetCollection<T>().UpdateManyAsync(filter, update);


        /// <summary>
        /// Takes documents you want to modify and applies the update you have defined in MongoDb with session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(Expression<Func<T, bool>> filter,
            UpdateDefinition<T> update,
            IClientSessionHandle session) =>
                await DbContext.GetCollection<T>().UpdateManyAsync(session, filter, update);


        /// <summary>
        /// Takes documents you want to modify and updates the field in entities with the given value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync<TField>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TField>> field,
            TField value) =>
                await DbContext.GetCollection<T>().UpdateManyAsync(filter, Builders<T>.Update.Set(field, value));


        /// <summary>
        /// Takes documents you want to modify and updates the field in entities with the given value with MongoDb session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync<TField>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TField>> field,
            TField value,
            IClientSessionHandle session) =>
                await DbContext.GetCollection<T>().UpdateManyAsync(session, filter, Builders<T>.Update.Set(field, value));


        /// <summary>
        /// Delete entity asynchronously
        /// </summary>
        /// <param name="entity">Db entity of type T</param>
        /// <param name="collectionName">Name of collection in db (by default is name of T)</param>
        public async Task DeleteAsync(T entity) =>
            await DbContext.GetCollection<T>().DeleteOneAsync(x => x.Id.Equals(entity.Id));


        /// <summary>
        /// Delete entity asynchronously with session
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="session"></param>
        /// <typeparam name="T">Type of collection</typeparam>
        /// <returns></returns>
        public async Task DeleteAsync(T entity, IClientSessionHandle session)
            => await DbContext.GetCollection<T>().DeleteOneAsync(session, x => x.Id.Equals(entity.Id));


        /// <summary>
        /// Deletes many entities by filter asynchronously
        /// </summary>
        /// <param name="filter">Filter for entity of type T</param>
        public async Task DeleteManyAsync(Expression<Func<T, bool>> filter) =>
            await DbContext.GetCollection<T>().DeleteManyAsync(filter);

        /// <summary>
        /// Counts documents by filter asynchronously
        /// </summary>
        /// <param name="filter">Filter for entity of type T</param>
        public async Task<long> CountAsync(Expression<Func<T, bool>> filter) =>
            await DbContext.GetCollection<T>().Find(filter).CountDocumentsAsync();
    }

}