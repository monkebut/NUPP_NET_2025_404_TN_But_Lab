using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure.Models
{
    /// <summary>
    /// Cartoon model - inherits from FilmModel using Table-per-Type
    /// </summary>
    [Table("Cartoons")]
    public class CartoonModel : IHasId
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

        [Required]
        [MaxLength(100)]
        public string Studio { get; set; } = string.Empty;

        [Required]
        public bool Is3D { get; set; }
    }
}

