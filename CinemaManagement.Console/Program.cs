using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaManagement.Common;

namespace CinemaManagement.ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа №2: Асинхронность и многопоточность ===\n");

            // Создаём асинхронные CRUD сервисы
            var movieService = new InMemoryCrudServiceAsync<Movie>("data/movies.json");
            var cartoonService = new InMemoryCrudServiceAsync<Cartoon>("data/cartoons.json");
            var ticketService = new InMemoryCrudServiceAsync<Ticket>("data/tickets.json");
            var customerService = new InMemoryCrudServiceAsync<Customer>("data/customers.json");
            var employeeService = new InMemoryCrudServiceAsync<Employee>("data/employees.json");

            // Демонстрация примитивов синхронизации
            await DemonstrateSynchronizationPrimitivesAsync();

            Console.WriteLine("\n=== Генерация объектов с помощью Parallel ===\n");

            var stopwatch = Stopwatch.StartNew();

            // Параллельное создание 1000+ фильмов
            Console.WriteLine("Создание 1500 фильмов...");
            Parallel.For(0, 1500, async i =>
            {
                var movie = Movie.CreateNew();
                await movieService.CreateAsync(movie);
            });

            // Параллельное создание мультфильмов
            Console.WriteLine("Создание 1200 мультфильмов...");
            Parallel.For(0, 1200, async i =>
            {
                var cartoon = Cartoon.CreateNew();
                await cartoonService.CreateAsync(cartoon);
            });

            // Параллельное создание билетов
            Console.WriteLine("Создание 2000 билетов...");
            Parallel.For(0, 2000, async i =>
            {
                var ticket = Ticket.CreateNew();
                await ticketService.CreateAsync(ticket);
            });

            // Параллельное создание клиентов
            Console.WriteLine("Создание 1000 клиентов...");
            Parallel.For(0, 1000, async i =>
            {
                var customer = Customer.CreateNew();
                await customerService.CreateAsync(customer);
            });

            // Параллельное создание сотрудников
            Console.WriteLine("Создание 500 сотрудников...");
            Parallel.For(0, 500, async i =>
            {
                var employee = Employee.CreateNew();
                await employeeService.CreateAsync(employee);
            });

            // Ждем завершения всех операций
            await Task.Delay(1000);

            stopwatch.Stop();
            Console.WriteLine($"\nВремя генерации: {stopwatch.ElapsedMilliseconds} мс\n");

            // LINQ аналитика для Movies
            Console.WriteLine("=== LINQ аналитика для фильмов (Movies) ===");
            var allMovies = await movieService.ReadAllAsync();
            var moviesList = allMovies.ToList();
            
            if (moviesList.Any())
            {
                Console.WriteLine($"Количество фильмов: {moviesList.Count}");
                Console.WriteLine($"Минимальная длительность: {moviesList.Min(m => m.Duration)} мин");
                Console.WriteLine($"Максимальная длительность: {moviesList.Max(m => m.Duration)} мин");
                Console.WriteLine($"Средняя длительность: {moviesList.Average(m => m.Duration):F2} мин");
                Console.WriteLine($"Минимальный бюджет: ${moviesList.Min(m => m.Budget):N0}");
                Console.WriteLine($"Максимальный бюджет: ${moviesList.Max(m => m.Budget):N0}");
                Console.WriteLine($"Средний бюджет: ${moviesList.Average(m => m.Budget):N0}");
            }

            // LINQ аналитика для Cartoons
            Console.WriteLine("\n=== LINQ аналитика для мультфильмов (Cartoons) ===");
            var allCartoons = await cartoonService.ReadAllAsync();
            var cartoonsList = allCartoons.ToList();
            
            if (cartoonsList.Any())
            {
                Console.WriteLine($"Количество мультфильмов: {cartoonsList.Count}");
                Console.WriteLine($"Минимальная длительность: {cartoonsList.Min(c => c.Duration)} мин");
                Console.WriteLine($"Максимальная длительность: {cartoonsList.Max(c => c.Duration)} мин");
                Console.WriteLine($"Средняя длительность: {cartoonsList.Average(c => c.Duration):F2} мин");
                Console.WriteLine($"Количество 3D мультфильмов: {cartoonsList.Count(c => c.Is3D)}");
            }

            // LINQ аналитика для Tickets
            Console.WriteLine("\n=== LINQ аналитика для билетов (Tickets) ===");
            var allTickets = await ticketService.ReadAllAsync();
            var ticketsList = allTickets.ToList();
            
            if (ticketsList.Any())
            {
                Console.WriteLine($"Количество билетов: {ticketsList.Count}");
                Console.WriteLine($"Минимальная цена: ${ticketsList.Min(t => t.Price):F2}");
                Console.WriteLine($"Максимальная цена: ${ticketsList.Max(t => t.Price):F2}");
                Console.WriteLine($"Средняя цена: ${ticketsList.Average(t => t.Price):F2}");
                Console.WriteLine($"Общая выручка: ${ticketsList.Sum(t => t.Price):N0}");
            }

            // LINQ аналитика для Customers
            Console.WriteLine("\n=== LINQ аналитика для клиентов (Customers) ===");
            var allCustomers = await customerService.ReadAllAsync();
            var customersList = allCustomers.ToList();
            
            if (customersList.Any())
            {
                Console.WriteLine($"Количество клиентов: {customersList.Count}");
                Console.WriteLine($"Минимальный возраст: {customersList.Min(c => c.Age)}");
                Console.WriteLine($"Максимальный возраст: {customersList.Max(c => c.Age)}");
                Console.WriteLine($"Средний возраст: {customersList.Average(c => c.Age):F2}");
            }

            // LINQ аналитика для Employees
            Console.WriteLine("\n=== LINQ аналитика для сотрудников (Employees) ===");
            var allEmployees = await employeeService.ReadAllAsync();
            var employeesList = allEmployees.ToList();
            
            if (employeesList.Any())
            {
                Console.WriteLine($"Количество сотрудников: {employeesList.Count}");
                Console.WriteLine($"Минимальный возраст: {employeesList.Min(e => e.Age)}");
                Console.WriteLine($"Максимальный возраст: {employeesList.Max(e => e.Age)}");
                Console.WriteLine($"Средний возраст: {employeesList.Average(e => e.Age):F2}");
                
                // Группировка по должности
                var byPosition = employeesList.GroupBy(e => e.Position)
                    .Select(g => new { Position = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count);
                
                Console.WriteLine("\nРаспределение по должностям:");
                foreach (var group in byPosition)
                {
                    Console.WriteLine($"  {group.Position}: {group.Count}");
                }
            }

            // Демонстрация пагинации
            Console.WriteLine("\n=== Демонстрация пагинации (первая страница по 5 элементов) ===");
            var firstPageMovies = await movieService.ReadAllAsync(1, 5);
            Console.WriteLine($"Первые 5 фильмов:");
            foreach (var movie in firstPageMovies)
            {
                Console.WriteLine($"  - {movie.Title} ({movie.Duration} мин, ${movie.Budget:N0})");
            }

            // Сохранение данных в файлы
            Console.WriteLine("\n=== Сохранение данных в файлы ===");
            await movieService.SaveAsync();
            Console.WriteLine($"Фильмы сохранены в {movieService.FilePath}");
            
            await cartoonService.SaveAsync();
            Console.WriteLine($"Мультфильмы сохранены в {cartoonService.FilePath}");
            
            await ticketService.SaveAsync();
            Console.WriteLine($"Билеты сохранены в {ticketService.FilePath}");
            
            await customerService.SaveAsync();
            Console.WriteLine($"Клиенты сохранены в {customerService.FilePath}");
            
            await employeeService.SaveAsync();
            Console.WriteLine($"Сотрудники сохранены в {employeeService.FilePath}");

            Console.WriteLine("\n=== Работа программы завершена ===");
        }

        // Демонстрация примитивов синхронизации
        static async Task DemonstrateSynchronizationPrimitivesAsync()
        {
            Console.WriteLine("=== Демонстрация примитивов синхронизации ===\n");

            // 1. Демонстрация lock
            Console.WriteLine("1. Демонстрация lock:");
            var lockObj = new object();
            int counter = 0;

            var lockTasks = Enumerable.Range(0, 10).Select(i => Task.Run(() =>
            {
                lock (lockObj)
                {
                    counter++;
                    Console.WriteLine($"   Lock: поток {Task.CurrentId} увеличил счетчик до {counter}");
                }
            }));

            await Task.WhenAll(lockTasks);
            Console.WriteLine($"   Финальное значение счетчика: {counter}\n");

            // 2. Демонстрация SemaphoreSlim
            Console.WriteLine("2. Демонстрация SemaphoreSlim (максимум 3 одновременных потока):");
            var semaphore = new SemaphoreSlim(3, 3);

            var semaphoreTasks = Enumerable.Range(0, 10).Select(i => Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                try
                {
                    Console.WriteLine($"   Semaphore: поток {Task.CurrentId} вошел в критическую секцию");
                    await Task.Delay(100);
                    Console.WriteLine($"   Semaphore: поток {Task.CurrentId} покинул критическую секцию");
                }
                finally
                {
                    semaphore.Release();
                }
            }));

            await Task.WhenAll(semaphoreTasks);
            Console.WriteLine();

            // 3. Демонстрация AutoResetEvent
            Console.WriteLine("3. Демонстрация AutoResetEvent:");
            var autoResetEvent = new AutoResetEvent(false);

            var autoResetTask = Task.Run(() =>
            {
                Console.WriteLine("   AutoResetEvent: поток ожидает сигнал...");
                autoResetEvent.WaitOne();
                Console.WriteLine("   AutoResetEvent: поток получил сигнал и продолжил работу");
            });

            await Task.Delay(500);
            Console.WriteLine("   AutoResetEvent: отправка сигнала...");
            autoResetEvent.Set();
            await autoResetTask;
            Console.WriteLine();

            // 4. Демонстрация ManualResetEvent
            Console.WriteLine("4. Демонстрация ManualResetEvent:");
            var manualResetEvent = new ManualResetEvent(false);

            var manualResetTasks = Enumerable.Range(0, 3).Select(i => Task.Run(() =>
            {
                Console.WriteLine($"   ManualResetEvent: поток {i + 1} ожидает сигнал...");
                manualResetEvent.WaitOne();
                Console.WriteLine($"   ManualResetEvent: поток {i + 1} получил сигнал");
            }));

            await Task.Delay(500);
            Console.WriteLine("   ManualResetEvent: отправка сигнала для всех потоков...");
            manualResetEvent.Set();
            await Task.WhenAll(manualResetTasks);
            Console.WriteLine();
        }
    }
}
