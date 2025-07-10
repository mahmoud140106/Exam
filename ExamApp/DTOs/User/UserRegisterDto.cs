using System.ComponentModel.DataAnnotations;

namespace ExamApp.DTOs.User
{
    public class UserRegisterDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }
}
