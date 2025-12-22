using System;

namespace CinemaManagement.Common
{
    // Наследник от Person
    public class Customer : Person
    {
        public string Email { get; set; }

        // Делегат и событие
        public delegate void NotificationHandler(string message);
        public event NotificationHandler? OnNotify;

        public Customer(string name, int age, string email)
            : base(name, age)
        {
            Email = email;
        }

        // Метод для вызова события
        public void Notify(string message)
        {
            OnNotify?.Invoke($"Уведомление для {Name}: {message}");
        }

        // Статический метод для создания случайного клиента
        public static Customer CreateNew()
        {
            var names = new[] { "Алексей", "Мария", "Иван", "Елена", "Дмитрий", "Анна", "Сергей", "Ольга" };
            var domains = new[] { "gmail.com", "yahoo.com", "outlook.com", "ukr.net" };

            var random = Random.Shared;
            var name = names[random.Next(names.Length)];
            var age = random.Next(18, 65);
            var email = $"{name.ToLower()}{random.Next(100, 999)}@{domains[random.Next(domains.Length)]}";

            return new Customer(name, age, email);
        }
    }
}
