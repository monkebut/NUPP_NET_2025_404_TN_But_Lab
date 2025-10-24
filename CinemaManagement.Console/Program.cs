using System;
using CinemaManagement.Common;

namespace CinemaManagement.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
            filmService.ReadAll().ShowAllFilms();

            //Создаём клиента и сотрудника
            var customer = new Customer("Алексей", 28, "alex@example.com");
            var employee = new Employee("Марина", 35, "Кассир");

            //Подписываемся на событие
            customer.OnNotify += (msg) => Console.WriteLine(msg);

            //Сотрудник обслуживает клиента
            employee.ServeCustomer(customer);

            //Создаём билет
            var ticket = new Ticket(customer, movie, 150);
            ticket.ShowTicketInfo();

            //Отправляем уведомление
            customer.Notify("Ваш билет успешно оформлен!");

            //Показываем общее количество фильмов
            Film.ShowTotalFilms();

            Console.WriteLine("\nРабота программы завершена");
        }
    }
}
