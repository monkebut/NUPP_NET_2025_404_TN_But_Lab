using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CinemaManagement.Common
{
    // Интерфейс CRUD
    public interface ICrudService<T>
    {
        void Create(T element);
        T Read(Guid id);
        IEnumerable<T> ReadAll();
        void Update(T element);
        void Remove(T element);
        void Load(string filePath);
        void Save(string filePath);
    }

    // Универсальный CRUD-сервис
    public class InMemoryCrudService<T> : ICrudService<T> where T : class
    {
        private readonly List<T> _storage = new List<T>();

        public void Create(T element) => _storage.Add(element);

        public T Read(Guid id)
        {
            return _storage.FirstOrDefault(e =>
                (Guid)e!.GetType().GetProperty("Id")!.GetValue(e)! == id)!;
        }

        public IEnumerable<T> ReadAll() => _storage;

        public void Update(T element)
        {
            var idProp = element.GetType().GetProperty("Id")!;
            Guid id = (Guid)idProp.GetValue(element)!;
            Remove(Read(id));
            Create(element);
        }

        public void Remove(T element) => _storage.Remove(element);

        public void Load(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}", filePath);

            try
            {
                var jsonString = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<List<T>>(jsonString);
                
                if (data != null)
                {
                    _storage.Clear();
                    _storage.AddRange(data);
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize data from file: {filePath}", ex);
            }
        }

        public void Save(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                var jsonString = JsonSerializer.Serialize(_storage, options);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save data to file: {filePath}", ex);
            }
        }
    }
}
