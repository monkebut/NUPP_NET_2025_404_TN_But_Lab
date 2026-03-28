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
    }
}
