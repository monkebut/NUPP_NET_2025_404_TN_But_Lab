using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaManagement.Common;
using MongoDB.Driver;

namespace CinemaManagement.MongoDb
{
    /// <summary>
    /// Generic MongoDB repository implementation
    /// </summary>
    /// <typeparam name="T">Entity type that implements IHasId</typeparam>
    public class MongoRepository<T> : IRepository<T> where T : class, IHasId
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            // Collection name based on type name
            var collectionName = typeof(T).Name;
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            try
            {
                var filter = Builders<T>.Filter.Eq(x => x.Id, id);
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }
            catch
            {
                return null;
            }
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            try
            {
                var filter = Builders<T>.Filter.Empty;
                return await _collection.Find(filter).ToListAsync();
            }
            catch
            {
                return new List<T>();
            }
        }

        public async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _collection.InsertOneAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            var options = new ReplaceOptions { IsUpsert = false };
            await _collection.ReplaceOneAsync(filter, entity, options);
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}

