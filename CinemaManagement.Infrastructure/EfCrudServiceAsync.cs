using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CinemaManagement.Common;
using Microsoft.EntityFrameworkCore;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// Entity Framework implementation of ICrudServiceAsync using repository pattern
    /// </summary>
    /// <typeparam name="T">Entity type that implements IHasId</typeparam>
    public class EfCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IHasId
    {
        private readonly IRepository<T> _repository;
        private readonly CinemaDbContext _context;

        public EfCrudServiceAsync(IRepository<T> repository, CinemaDbContext context)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
            try
            {
                var result = await _repository.GetByIdAsync(id);
                return result ?? null!;
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
                return await _repository.GetAllAsync();
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
                if (page < 1 || amount < 1)
                    return Enumerable.Empty<T>();

                var allItems = await _repository.GetAllAsync();
                return allItems
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
                await _repository.Update(element);
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
                await _repository.Delete(element);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Реалізація IEnumerable<T>
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
