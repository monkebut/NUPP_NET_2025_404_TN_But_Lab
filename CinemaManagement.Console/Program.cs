using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaManagement.Common;
using CinemaManagement.MongoDb;

namespace CinemaManagement.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа №3: База данных MongoDB ===\n");

            try
            {
                // Initialize MongoDB context
                Console.WriteLine("Подключение к MongoDB...");
                var context = new MongoDbContext();
                
                // Create repository and CRUD service for Movies
                var movieRepository = new MongoRepository<Movie>(context.Database);
                var movieService = new MongoCrudServiceAsync<Movie>(movieRepository);

                Console.WriteLine($"Подключено к базе данных: {context.Database.DatabaseNamespace.DatabaseName}");
                Console.WriteLine();

                // Scenario A: Parallel Bulk Insert with 1000+ objects
                await ScenarioA_ParallelBulkInsert(movieService);

                // Scenario B: LINQ Analytics
                await ScenarioB_LinqAnalytics(movieService);

                // Scenario C: Single CRUD Operations
                await ScenarioC_CrudOperations(movieService);

                // Scenario D: Pagination Demo
                await ScenarioD_Pagination(movieService);

                Console.WriteLine("\n=== Работа программы завершена успешно ===");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("MongoDB"))
            {
                Console.WriteLine("\n❌ ОШИБКА ПОДКЛЮЧЕНИЯ К MONGODB:");
                Console.WriteLine($"   {ex.Message}");
                Console.WriteLine("\nУбедитесь, что:");
                Console.WriteLine("  1. MongoDB запущен (mongodb://localhost:27017)");
                Console.WriteLine("  2. Переменные окружения настроены правильно:");
                Console.WriteLine("     - MONGO_CONNECTION (по умолчанию: mongodb://localhost:27017)");
                Console.WriteLine("     - MONGO_DB (по умолчанию: CinemaManagementDb)");
                Console.WriteLine("\nПрограма завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ НЕПРЕДВИДЕННАЯ ОШИБКА: {ex.Message}");
                Console.WriteLine("Программа завершена.");
            }
        }

        /// <summary>
        /// Scenario A: Parallel bulk insert of 1000+ movies with controlled parallelism
        /// </summary>
        static async Task ScenarioA_ParallelBulkInsert(ICrudServiceAsync<Movie> movieService)
        {
            Console.WriteLine("=== СЦЕНАРИЙ A: Параллельная массовая вставка ===\n");

            const int movieCount = 100; // Зменшено для швидшого тесту з cloud MongoDB
            const int maxParallelism = 30;

            Console.WriteLine($"Создание и вставка {movieCount} фильмов с ограничением параллелизма {maxParallelism}...");
            
            var stopwatch = Stopwatch.StartNew();
            var semaphore = new SemaphoreSlim(maxParallelism, maxParallelism);
            var tasks = new Task[movieCount];

            for (int i = 0; i < movieCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var movie = Movie.CreateNew();
                        await movieService.CreateAsync(movie);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }

            await Task.WhenAll(tasks);
            stopwatch.Stop();

            Console.WriteLine($"✓ Вставлено {movieCount} фильмов за {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine($"  Средняя скорость: {movieCount * 1000.0 / stopwatch.ElapsedMilliseconds:F2} записей/сек\n");
        }

        /// <summary>
        /// Scenario B: LINQ analytics on all movies
        /// </summary>
        static async Task ScenarioB_LinqAnalytics(ICrudServiceAsync<Movie> movieService)
        {
            Console.WriteLine("=== СЦЕНАРИЙ B: LINQ аналитика ===\n");

            Console.WriteLine("Загрузка всех фильмов из MongoDB...");
            var allMovies = await movieService.ReadAllAsync();
            var moviesList = allMovies.ToList();

            if (moviesList.Any())
            {
                Console.WriteLine($"✓ Загружено фильмов: {moviesList.Count}\n");

                Console.WriteLine("Анализ длительности (Duration):");
                Console.WriteLine($"  Минимальная: {moviesList.Min(m => m.Duration)} мин");
                Console.WriteLine($"  Максимальная: {moviesList.Max(m => m.Duration)} мин");
                Console.WriteLine($"  Средняя: {moviesList.Average(m => m.Duration):F2} мин\n");

                Console.WriteLine("Анализ бюджета (Budget):");
                Console.WriteLine($"  Минимальный: ${moviesList.Min(m => m.Budget):N0}");
                Console.WriteLine($"  Максимальный: ${moviesList.Max(m => m.Budget):N0}");
                Console.WriteLine($"  Средний: ${moviesList.Average(m => m.Budget):N0}\n");

                // Additional analytics
                var totalBudget = moviesList.Sum(m => m.Budget);
                Console.WriteLine($"Общий бюджет всех фильмов: ${totalBudget:N0}\n");
            }
            else
            {
                Console.WriteLine("⚠ Нет данных для анализа\n");
            }
        }

        /// <summary>
        /// Scenario C: CRUD operations on a single movie
        /// </summary>
        static async Task ScenarioC_CrudOperations(ICrudServiceAsync<Movie> movieService)
        {
            Console.WriteLine("=== СЦЕНАРИЙ C: CRUD операции на одном элементе ===\n");

            // Get a movie ID from the database
            var allMovies = await movieService.ReadAllAsync();
            var firstMovie = allMovies.FirstOrDefault();

            if (firstMovie == null)
            {
                Console.WriteLine("⚠ Нет фильмов в базе данных для демонстрации CRUD\n");
                return;
            }

            var movieId = firstMovie.Id;
            Console.WriteLine($"Работа с фильмом ID: {movieId}\n");

            // READ operation
            Console.WriteLine("1. READ - Чтение фильма:");
            var movie = await movieService.ReadAsync(movieId);
            if (movie != null)
            {
                Console.WriteLine($"   ✓ Прочитан: {movie.Title}, Жанр: {movie.Genre}, Бюджет: ${movie.Budget:N0}");
            }
            else
            {
                Console.WriteLine("   ✗ Фильм не найден");
                return;
            }

            // UPDATE operation
            Console.WriteLine("\n2. UPDATE - Обновление фильма:");
            var originalTitle = movie.Title;
            var originalBudget = movie.Budget;
            movie.Title = movie.Title + " (Обновлено)";
            movie.Budget = movie.Budget * 1.5;
            
            var updateResult = await movieService.UpdateAsync(movie);
            if (updateResult)
            {
                Console.WriteLine($"   ✓ Обновлено: {originalTitle} -> {movie.Title}");
                Console.WriteLine($"   ✓ Бюджет изменен: ${originalBudget:N0} -> ${movie.Budget:N0}");
                
                // Verify update
                var updatedMovie = await movieService.ReadAsync(movieId);
                if (updatedMovie != null && updatedMovie.Title.Contains("Обновлено"))
                {
                    Console.WriteLine("   ✓ Проверка: обновление применено успешно");
                }
            }
            else
            {
                Console.WriteLine("   ✗ Ошибка обновления");
            }

            // DELETE operation
            Console.WriteLine("\n3. REMOVE - Удаление фильма:");
            var deleteResult = await movieService.RemoveAsync(movie);
            if (deleteResult)
            {
                Console.WriteLine($"   ✓ Удален: {movie.Title}");
                
                // Verify deletion
                var deletedMovie = await movieService.ReadAsync(movieId);
                if (deletedMovie == null)
                {
                    Console.WriteLine("   ✓ Проверка: фильм удален из базы данных");
                }
            }
            else
            {
                Console.WriteLine("   ✗ Ошибка удаления");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Scenario D: Pagination demonstration
        /// </summary>
        static async Task ScenarioD_Pagination(ICrudServiceAsync<Movie> movieService)
        {
            Console.WriteLine("=== СЦЕНАРИЙ D: Демонстрация пагинации ===\n");

            const int pageSize = 10;

            // First page
            Console.WriteLine($"Страница 1 (первые {pageSize} фильмов):");
            var page1 = await movieService.ReadAllAsync(1, pageSize);
            var page1List = page1.ToList();
            Console.WriteLine($"  Получено элементов: {page1List.Count}");
            
            if (page1List.Any())
            {
                for (int i = 0; i < Math.Min(3, page1List.Count); i++)
                {
                    var movie = page1List[i];
                    Console.WriteLine($"    {i + 1}. {movie.Title} ({movie.Duration} мин, ${movie.Budget:N0})");
                }
                if (page1List.Count > 3)
                {
                    Console.WriteLine($"    ... и еще {page1List.Count - 3} фильмов");
                }
            }

            Console.WriteLine();

            // Second page
            Console.WriteLine($"Страница 2 (следующие {pageSize} фильмов):");
            var page2 = await movieService.ReadAllAsync(2, pageSize);
            var page2List = page2.ToList();
            Console.WriteLine($"  Получено элементов: {page2List.Count}");
            
            if (page2List.Any())
            {
                for (int i = 0; i < Math.Min(3, page2List.Count); i++)
                {
                    var movie = page2List[i];
                    Console.WriteLine($"    {i + 1}. {movie.Title} ({movie.Duration} мин, ${movie.Budget:N0})");
                }
                if (page2List.Count > 3)
                {
                    Console.WriteLine($"    ... и еще {page2List.Count - 3} фильмов");
                }
            }

            Console.WriteLine();

            // Test invalid parameters
            Console.WriteLine("Проверка некорректных параметров пагинации:");
            var invalidPage = await movieService.ReadAllAsync(-1, 10);
            Console.WriteLine($"  ReadAllAsync(-1, 10): {invalidPage.Count()} элементов (ожидается 0)");
            
            var invalidAmount = await movieService.ReadAllAsync(1, -5);
            Console.WriteLine($"  ReadAllAsync(1, -5): {invalidAmount.Count()} элементов (ожидается 0)");

            Console.WriteLine();
        }
    }
}
