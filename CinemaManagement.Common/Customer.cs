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
    }
}
