using System;
using CinemaManagement.Common;
using System.IO;

namespace CinemaManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   ЛАБОРАТОРНА РОБОТА 1");
            Console.WriteLine("   Додаткове завдання: Робота з JSON");
            Console.WriteLine("========================================\n");

            //Создаём CRUD сервисы
            var filmService = new InMemoryCrudService<Film>();
            var customerService = new InMemoryCrudService<Customer>();

            //Создаём фильмы
            var movie = new Movie("Интерстеллар", "Фантастика", 169, "Кристофер Нолан", 165000000);
            var cartoon = new Cartoon("Шрек", "Комедия", 90, "DreamWorks", true);

            //Добавляем фильмы в CRUD
            filmService.Create(movie);
            filmService.Create(cartoon);

            //Показываем список фильмов
            Console.WriteLine("\n--- Початковий список фільмів ---");
            filmService.ReadAll().ShowAllFilms();

            //Создаём клиента и сотрудника
            var customer = new Customer("Алексей", 28, "alex@example.com");
            var employee = new Employee("Марина", 35, "Кассир");

            //Подписываемся на событие
            customer.OnNotify += (msg) => Console.WriteLine($"🔔 {msg}");

            //Сотрудник обслуживает клиента
            employee.ServeCustomer(customer);

            //Создаём билет
            var ticket = new Ticket(customer, movie, 150);
            ticket.ShowTicketInfo();

            //Отправляем уведомление
            customer.Notify("Ваш билет успешно оформлен!");

            //Показываем общее количество фильмов
            Film.ShowTotalFilms();

            // ========== ДОДАТКОВЕ ЗАВДАННЯ: Робота з JSON файлами ==========
            Console.WriteLine("\n\n========================================");
            Console.WriteLine("   ДОДАТКОВЕ ЗАВДАННЯ: Робота з JSON");
            Console.WriteLine("========================================\n");

            // Визначаємо шлях до папки data (відносно кореня проекту)
            var currentDir = Directory.GetCurrentDirectory();
            var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", ".."));
            var dataPath = Path.Combine(projectRoot, "data");
            var moviesFilePath = Path.Combine(dataPath, "movies.json");
            var cartoonsFilePath = Path.Combine(dataPath, "cartoons.json");
            var outputPath = Path.Combine(currentDir, "output");

            try
            {
                // 1. Завантаження фільмів з JSON файлу
                Console.WriteLine("Завантаження фільмів з JSON файлу...");
                if (File.Exists(moviesFilePath))
                {
                    var loadedMovieService = new InMemoryCrudService<Movie>();
                    loadedMovieService.Load(moviesFilePath);
                    
                    Console.WriteLine($"Завантажено {loadedMovieService.ReadAll().Count()} фільмів з файлу");
                    Console.WriteLine("\n--- Перші 3 фільми з файлу ---");
                    foreach (var loadedMovie in loadedMovieService.ReadAll().Take(3))
                    {
                        loadedMovie.ShowInfo();
                    }
                }
                else
                {
                    Console.WriteLine($"Файл не знайдено: {moviesFilePath}");
                }

                // 2. Завантаження мультфільмів з JSON файлу
                Console.WriteLine("\nЗавантаження мультфільмів з JSON файлу...");
                if (File.Exists(cartoonsFilePath))
                {
                    var loadedCartoonService = new InMemoryCrudService<Cartoon>();
                    loadedCartoonService.Load(cartoonsFilePath);
                    
                    Console.WriteLine($"Завантажено {loadedCartoonService.ReadAll().Count()} мультфільмів з файлу");
                    Console.WriteLine("\n--- Перші 3 мультфільми з файлу ---");
                    foreach (var loadedCartoon in loadedCartoonService.ReadAll().Take(3))
                    {
                        loadedCartoon.ShowInfo();
                    }
                }
                else
                {
                    Console.WriteLine($"Файл не знайдено: {cartoonsFilePath}");
                }

                // 3. Збереження поточних даних у JSON файл
                Console.WriteLine("\nЗбереження поточних даних у JSON файл...");
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                var outputMoviesFile = Path.Combine(outputPath, "saved_movies.json");
                filmService.Save(outputMoviesFile);
                Console.WriteLine($"Дані збережено у файл: {outputMoviesFile}");

                // 4. Демонстрація роботи з файлами
                Console.WriteLine("\nСтатистика:");
                Console.WriteLine($"   - Фільмів у пам'яті: {filmService.ReadAll().Count()}");
                if (File.Exists(outputMoviesFile))
                {
                    var fileInfo = new FileInfo(outputMoviesFile);
                    Console.WriteLine($"   - Розмір збереженого файлу: {fileInfo.Length} байт");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при роботі з файлами: {ex.Message}");
            }

            Console.WriteLine("\n========================================");
            Console.WriteLine("Работа программы завершена");
            Console.WriteLine("========================================");
            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}
