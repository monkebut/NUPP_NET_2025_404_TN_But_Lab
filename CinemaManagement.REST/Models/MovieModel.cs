using System;

namespace CinemaManagement.REST.Models
{
    /// <summary>
    /// DTO for reading Movie data
    /// </summary>
    public class MovieModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Director { get; set; } = string.Empty;
        public double Budget { get; set; }
    }
}


