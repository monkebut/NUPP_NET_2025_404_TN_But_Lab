using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure.Models
{
    /// <summary>
    /// Customer model - inherits from PersonModel using Table-per-Type
    /// </summary>
    [Table("Customers")]
    public class CustomerModel : IHasId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Navigation property for tickets (one-to-many)
        public virtual ICollection<TicketModel> Tickets { get; set; } = new List<TicketModel>();
    }
}

