using System;
using System.Collections.Generic;
using System.Linq;

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
    }
}
