using System;
using System.Collections.Generic;

namespace CinemaManagement.Common
{
    // Базовый класс фильма
    public class Film : IHasId
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public int Duration { get; set; } // в минутах

        // Статическое поле — общее количество фильмов
        public static int TotalFilms;

        // Статический конструктор
        static Film()
        {
            TotalFilms = 0;
        }

        // Конструктор
        public Film(string title, string genre, int duration)
        {
            Id = Guid.NewGuid();
            Title = title;
            Genre = genre;
            Duration = duration;
            TotalFilms++;
        }

        // Метод
        public virtual void ShowInfo()
        {
            Console.WriteLine($"Фильм: {Title}, Жанр: {Genre}, Длительность: {Duration} мин.");
        }

        // Статический метод
        public static void ShowTotalFilms()
        {
            Console.WriteLine($"Всего фильмов: {TotalFilms}");
        }
    }
}
