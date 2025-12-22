using System.ComponentModel.DataAnnotations;

namespace CinemaManagement.REST.Models
{
    /// <summary>
    /// DTO for creating a new Movie
    /// </summary>
    public class MovieCreateModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(100, ErrorMessage = "Genre cannot exceed 100 characters")]
        public string Genre { get; set; } = string.Empty;

        [Required]
        [Range(1, 500, ErrorMessage = "Duration must be between 1 and 500 minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Director is required")]
        [StringLength(200, ErrorMessage = "Director name cannot exceed 200 characters")]
        public string Director { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Budget must be a positive value")]
        public double Budget { get; set; }
    }
}

