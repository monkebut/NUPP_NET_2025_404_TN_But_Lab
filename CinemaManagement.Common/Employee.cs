using System;

namespace CinemaManagement.Common
{
    // Наследник от Person
    public class Employee : Person
    {
        public string Position { get; set; }

        public Employee(string name, int age, string position)
            : base(name, age)
        {
            Position = position;
        }

        public void ServeCustomer(Customer customer)
        {
            Console.WriteLine($"{Name} ({Position}) обслуживает клиента {customer.Name}");
        }

        // Статический метод для создания случайного сотрудника
        public static Employee CreateNew()
        {
            var names = new[] { "Марина", "Виктор", "Наталья", "Андрей", "Светлана", "Павел" };
            var positions = new[] { "Кассир", "Администратор", "Уборщик", "Контролер", "Менеджер" };

            var random = Random.Shared;
            var name = names[random.Next(names.Length)];
            var age = random.Next(20, 60);
            var position = positions[random.Next(positions.Length)];

            return new Employee(name, age, position);
        }
    }
}
