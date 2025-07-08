using ExamApp.Helpers;
using ExamApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null!;

            return JwtHelper.GenerateToken(user, _config);
        }
    }

}
