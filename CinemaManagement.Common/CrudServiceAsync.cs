using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaManagement.Common
{
    // Асинхронний інтерфейс CRUD
    public interface ICrudServiceAsync<T> : IEnumerable<T>
    {
        Task<bool> CreateAsync(T element);
        Task<T> ReadAsync(Guid id);
        Task<IEnumerable<T>> ReadAllAsync();
        Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
        Task<bool> UpdateAsync(T element);
        Task<bool> RemoveAsync(T element);
        Task<bool> SaveAsync();
    }

    // Асинхронний thread-safe CRUD-сервіс з підтримкою пагінації та серіалізації
    public class InMemoryCrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly ConcurrentDictionary<Guid, T> _storage = new();
        private readonly SemaphoreSlim _fileSemaphore = new(1, 1);
        private readonly string _filePath;

        public string FilePath => _filePath;

        public InMemoryCrudServiceAsync(string filePath)
        {
            _filePath = filePath;
        }

        public async Task<bool> CreateAsync(T element)
        {
            await Task.Yield(); // Імітація асинхронної операції
            
            var idProperty = element.GetType().GetProperty("Id");
            if (idProperty == null)
                return false;

            var id = (Guid)idProperty.GetValue(element)!;
            return _storage.TryAdd(id, element);
        }

        public async Task<T> ReadAsync(Guid id)
        {
            await Task.Yield();
            _storage.TryGetValue(id, out var element);
            return element!;
        }

        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            await Task.Yield();
            return _storage.Values.ToList();
        }

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            await Task.Yield();
            
            if (page < 1 || amount < 1)
                return Enumerable.Empty<T>();

            return _storage.Values
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList();
        }

        public async Task<bool> UpdateAsync(T element)
        {
            await Task.Yield();
            
            var idProperty = element.GetType().GetProperty("Id");
            if (idProperty == null)
                return false;

            var id = (Guid)idProperty.GetValue(element)!;
            
            if (!_storage.ContainsKey(id))
                return false;

            _storage[id] = element;
            return true;
        }

        public async Task<bool> RemoveAsync(T element)
        {
            await Task.Yield();
            
            var idProperty = element.GetType().GetProperty("Id");
            if (idProperty == null)
                return false;

            var id = (Guid)idProperty.GetValue(element)!;
            return _storage.TryRemove(id, out _);
        }

        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var data = _storage.Values.ToList();
                var options = new JsonSerializerOptions { WriteIndented = true };
                
                await using var fileStream = File.Create(_filePath);
                await JsonSerializer.SerializeAsync(fileStream, data, options);
                
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        // Реалізація IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

