using ExamApp.Helpers;
using ExamApp.Services.Interface;
using ExamApp.UnitOfWork;

namespace ExamApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.UserRepo.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return JwtHelper.GenerateToken(user, _config);
        }
    }
}
