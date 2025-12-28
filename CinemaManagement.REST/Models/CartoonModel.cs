using System;

namespace CinemaManagement.REST.Models
{
    /// <summary>
    /// DTO for reading Cartoon data
    /// </summary>
    public class CartoonModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Studio { get; set; } = string.Empty;
        public bool Is3D { get; set; }
    }
}


