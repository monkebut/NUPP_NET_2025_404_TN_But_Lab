using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure
{
    public class EfCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IHasId
    {
        private readonly IRepository<T> _repository;

        public EfCrudServiceAsync(IRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<bool> CreateAsync(T element)
        {
            try
            {
                await _repository.AddAsync(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<T> ReadAsync(Guid id)
        {
            var result = await _repository.GetByIdAsync(id);
            return result ?? null!;
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1 || amount < 1)
                return Enumerable.Empty<T>();

            var allItems = await _repository.GetAllAsync();
            return allItems
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList();
        }

        public async Task<bool> UpdateAsync(T element)
        {
            try
            {
                await _repository.UpdateAsync(element);
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            return await Task.FromResult(true);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var items = _repository.GetAllAsync().Result;
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
