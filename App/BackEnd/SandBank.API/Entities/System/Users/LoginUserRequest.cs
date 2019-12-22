using System.ComponentModel.DataAnnotations;

namespace Entities.System.Users
{
    public class LoginUserRequest
    {
        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }
            
        [Required]
        public string Password { get; set; }
    }
}