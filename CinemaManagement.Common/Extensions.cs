using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaManagement.Common
{
    // Метод расширения
    public static class Extensions
    {
        public static void ShowAllFilms(this IEnumerable<Film> films)
        {
            Console.WriteLine("------ Список фильмов ------");
            foreach (var film in films)
                film.ShowInfo();
            Console.WriteLine("----------------------------");
        }
    }
}
