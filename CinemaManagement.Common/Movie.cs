using System;

namespace CinemaManagement.Common
{
    // Наследник от Film
    public class Movie : Film
    {
        public string Director { get; set; }
        public double Budget { get; set; }

        // Конструктор
        public Movie(string title, string genre, int duration, string director, double budget)
            : base(title, genre, duration)
        {
            Director = director;
            Budget = budget;
        }

        // Переопределение метода
        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Режиссёр: {Director}, Бюджет: {Budget}$");
        }
    }
}
