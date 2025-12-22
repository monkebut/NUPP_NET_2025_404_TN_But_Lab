using System.ComponentModel.DataAnnotations;

namespace CinemaManagement.REST.Models
{
    /// <summary>
    /// DTO for updating an existing Cartoon
    /// </summary>
    public class CartoonUpdateModel
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

        [Required(ErrorMessage = "Studio is required")]
        [StringLength(200, ErrorMessage = "Studio name cannot exceed 200 characters")]
        public string Studio { get; set; } = string.Empty;

        public bool Is3D { get; set; }
    }
}

