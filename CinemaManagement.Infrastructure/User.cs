using Microsoft.AspNetCore.Identity;

namespace CinemaManagement.Infrastructure
{
    /// <summary>
    /// User entity that inherits from IdentityUser
    /// </summary>
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}


