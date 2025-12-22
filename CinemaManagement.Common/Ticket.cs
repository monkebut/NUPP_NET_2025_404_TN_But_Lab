using System;

namespace CinemaManagement.Common
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Customer Customer { get; set; }
        public Film Film { get; set; }
        public double Price { get; set; }

        public Ticket(Customer customer, Film film, double price)
        {
            Id = Guid.NewGuid();
            Customer = customer;
            Film = film;
            Price = price;
        }

        public void ShowTicketInfo()
        {
            Console.WriteLine($"Билет: {Film.Title}, Цена: {Price}$, Покупатель: {Customer.Name}");
        }

        // Статический метод для создания случайного билета
        public static Ticket CreateNew()
        {
            var customer = Customer.CreateNew();
            var random = Random.Shared;
            
            // Создаем случайный фильм или мультфильм
            Film film = random.Next(2) == 0 
                ? (Film)Movie.CreateNew() 
                : (Film)Cartoon.CreateNew();
            
            var price = random.Next(100, 500);

            return new Ticket(customer, film, price);
        }
    }
}
