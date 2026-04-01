using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;

namespace CinemaManagement.MongoDb
{
    /// <summary>
    /// MongoDB-backed implementation of ICrudServiceAsync
    /// </summary>
    /// <typeparam name="T">Entity type that implements IHasId</typeparam>
    public class MongoCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IHasId
    {
        private readonly IRepository<T> _repository;
        private List<T> _cache;

        public MongoCrudServiceAsync(IRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _cache = new List<T>();
            
            // Initialize cache asynchronously (fire and forget for constructor)
            _ = RefreshCacheAsync();
        }

        private async Task RefreshCacheAsync()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                _cache = items.ToList();
            }
            catch
            {
                _cache = new List<T>();
            }
        }

        public async Task<bool> CreateAsync(T element)
        {
            try
            {
                await _repository.AddAsync(element);
                await RefreshCacheAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id);
                return result!;
            }
            catch
            {
                return null!;
            }
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            try
            {
                var items = await _repository.GetAllAsync();
                await RefreshCacheAsync();
                return items;
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            try
            {
                // Validate parameters
                if (page < 1 || amount < 1)
                    return Enumerable.Empty<T>();

                var allItems = await _repository.GetAllAsync();
                
                // Order by Id for consistency, then paginate
                return allItems
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * amount)
                    .Take(amount)
                    .ToList();
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }

        public async Task<bool> UpdateAsync(T element)
        {
            try
            {
                await _repository.UpdateAsync(element);
                await RefreshCacheAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveAsync(T element)
        {
            try
            {
                await _repository.DeleteAsync(element);
                await RefreshCacheAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            // No-op for MongoDB (changes are persisted immediately)
            await Task.CompletedTask;
            return true;
        }

        // IEnumerable implementation using cached snapshot
        public IEnumerator<T> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

