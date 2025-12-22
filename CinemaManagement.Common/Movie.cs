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

        // Статический метод для создания случайного фильма
        public static Movie CreateNew()
        {
            var titles = new[] { "Интерстеллар", "Начало", "Матрица", "Темный рыцарь", "Бойцовский клуб", "Форрест Гамп", "Криминальное чтиво", "Зеленая миля" };
            var genres = new[] { "Фантастика", "Боевик", "Драма", "Триллер", "Комедия" };
            var directors = new[] { "Кристофер Нолан", "Стивен Спилберг", "Квентин Тарантино", "Мартин Скорсезе", "Ридли Скотт" };

            var random = Random.Shared;
            var title = titles[random.Next(titles.Length)];
            var genre = genres[random.Next(genres.Length)];
            var duration = random.Next(90, 180);
            var director = directors[random.Next(directors.Length)];
            var budget = random.Next(10000000, 300000000);

            return new Movie(title, genre, duration, director, budget);
        }
    }
}
