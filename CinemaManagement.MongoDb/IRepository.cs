using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CinemaManagement.Common;

namespace CinemaManagement.MongoDb
{
    /// <summary>
    /// Generic repository interface for MongoDB operations
    /// </summary>
    /// <typeparam name="T">Entity type that implements IHasId</typeparam>
    public interface IRepository<T> where T : class, IHasId
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}

