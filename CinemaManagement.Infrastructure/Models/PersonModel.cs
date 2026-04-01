using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CinemaManagement.Common;

namespace CinemaManagement.Infrastructure.Models
{
    /// <summary>
    /// Base model for Person entity using Table-per-Type inheritance
    /// </summary>
    [Table("Persons")]
    public class PersonModel : IHasId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }
    }
}

