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
    }
}
