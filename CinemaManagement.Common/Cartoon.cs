using System;

namespace CinemaManagement.Common
{
    // Наследник от Film
    public class Cartoon : Film
    {
        public string Studio { get; set; }
        public bool Is3D { get; set; }

        public Cartoon(string title, string genre, int duration, string studio, bool is3D)
            : base(title, genre, duration)
        {
            Studio = studio;
            Is3D = is3D;
        }

        public override void ShowInfo()
        {
            base.ShowInfo();
            Console.WriteLine($"Студия: {Studio}, 3D: {(Is3D ? "Да" : "Нет")}");
        }

        // Статический метод для создания случайного мультфильма
        public static Cartoon CreateNew()
        {
            var titles = new[] { "Шрек", "История игрушек", "Как приручить дракона", "Корпорация монстров", "В поисках Немо", "Тачки", "Рататуй" };
            var genres = new[] { "Комедия", "Приключения", "Семейный", "Фэнтези" };
            var studios = new[] { "DreamWorks", "Pixar", "Disney", "Illumination", "Blue Sky" };

            var random = Random.Shared;
            var title = titles[random.Next(titles.Length)];
            var genre = genres[random.Next(genres.Length)];
            var duration = random.Next(75, 120);
            var studio = studios[random.Next(studios.Length)];
            var is3D = random.Next(2) == 1;

            return new Cartoon(title, genre, duration, studio, is3D);
        }
    }
}
