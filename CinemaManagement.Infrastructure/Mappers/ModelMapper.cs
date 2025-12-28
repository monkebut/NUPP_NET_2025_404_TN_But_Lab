using System;
using CinemaManagement.Common;
using CinemaManagement.Infrastructure.Models;

namespace CinemaManagement.Infrastructure.Mappers
{
    /// <summary>
    /// Mapper for converting between Common models and Infrastructure models
    /// </summary>
    public static class ModelMapper
    {
        public static CustomerModel ToModel(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Age = customer.Age,
                Email = customer.Email
            };
        }

        public static Customer FromModel(CustomerModel model)
        {
            return new Customer(model.Name, model.Age, model.Email)
            {
                Id = model.Id
            };
        }

        public static EmployeeModel ToModel(Employee employee)
        {
            return new EmployeeModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Age = employee.Age,
                Position = employee.Position
            };
        }

        public static Employee FromModel(EmployeeModel model)
        {
            return new Employee(model.Name, model.Age, model.Position)
            {
                Id = model.Id
            };
        }

        public static MovieModel ToModel(Movie movie)
        {
            return new MovieModel
            {
                Id = movie.Id,
                Title = movie.Title,
                Genre = movie.Genre,
                Duration = movie.Duration,
                Director = movie.Director,
                Budget = (decimal)movie.Budget
            };
        }

        public static Movie FromModel(MovieModel model)
        {
            return new Movie(model.Title, model.Genre, model.Duration, model.Director, (double)model.Budget)
            {
                Id = model.Id
            };
        }

        public static CartoonModel ToModel(Cartoon cartoon)
        {
            return new CartoonModel
            {
                Id = cartoon.Id,
                Title = cartoon.Title,
                Genre = cartoon.Genre,
                Duration = cartoon.Duration,
                Studio = cartoon.Studio,
                Is3D = cartoon.Is3D
            };
        }

        public static Cartoon FromModel(CartoonModel model)
        {
            return new Cartoon(model.Title, model.Genre, model.Duration, model.Studio, model.Is3D)
            {
                Id = model.Id
            };
        }

        public static FilmModel ToModel(Film film)
        {
            return new FilmModel
            {
                Id = film.Id,
                Title = film.Title,
                Genre = film.Genre,
                Duration = film.Duration
            };
        }

        public static Film FromModel(FilmModel model)
        {
            return new Film(model.Title, model.Genre, model.Duration)
            {
                Id = model.Id
            };
        }

        public static TicketModel ToModel(Ticket ticket)
        {
            return new TicketModel
            {
                Id = ticket.Id,
                CustomerId = ticket.Customer.Id,
                FilmId = ticket.Film.Id,
                Price = (decimal)ticket.Price
            };
        }

        public static Ticket FromModel(TicketModel model, Customer customer, Film film)
        {
            return new Ticket(customer, film, (double)model.Price)
            {
                Id = model.Id
            };
        }
    }
}

