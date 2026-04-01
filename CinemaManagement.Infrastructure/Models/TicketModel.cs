using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure.Models
{
    /// <summary>
    /// Ticket model with relationships to Customer and Film
    /// </summary>
    [Table("Tickets")]
    public class TicketModel : IHasId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid FilmId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Navigation properties (one-to-many relationships)
        [ForeignKey(nameof(CustomerId))]
        public virtual CustomerModel? Customer { get; set; }

        [ForeignKey(nameof(FilmId))]
        public virtual FilmModel? Film { get; set; }
    }
}

