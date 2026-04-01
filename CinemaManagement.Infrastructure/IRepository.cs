using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// Generic repository interface for data operations
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
