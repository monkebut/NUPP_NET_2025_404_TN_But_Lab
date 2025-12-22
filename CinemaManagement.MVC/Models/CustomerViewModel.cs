using System;
using System.ComponentModel.DataAnnotations;

namespace CinemaManagement.MVC.Models
{
    /// <summary>
    /// ViewModel for displaying customer information
    /// </summary>
    public class CustomerViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Customer Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Age")]
        public int Age { get; set; }

        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }
}

