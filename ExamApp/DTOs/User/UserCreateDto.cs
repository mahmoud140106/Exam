using System.ComponentModel.DataAnnotations;

namespace ExamApp.DTOs.User
{
    public class UserCreateDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;
    }

}
