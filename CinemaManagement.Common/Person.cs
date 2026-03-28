using System;

namespace CinemaManagement.Common
{
    // Базовый класс человека
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Id = Guid.NewGuid();
            Name = name;
            Age = age;
        }
    }
}
