using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure.Models
{
    /// <summary>
    /// Base model for Film entity using Table-per-Type inheritance
    /// </summary>
    [Table("Films")]
    public class FilmModel : IHasId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [Required]
        public int Duration { get; set; }

        // Navigation property for tickets (one-to-many)
        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();
    }
}

