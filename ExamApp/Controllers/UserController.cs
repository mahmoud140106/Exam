using AutoMapper;
using ExamApp.DTOs.User;
using ExamApp.Helpers;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _unitOfWork.UserRepo.IsEmailExistsAsync(dto.Email))
                return Fail("Email already exists.");

            if (await _unitOfWork.UserRepo.IsUsernameExistsAsync(dto.Username))
                return Fail("Username already exists.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "Student";

            await _unitOfWork.UserRepo.RegisterAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<UserResponseDto>(user);
            return Success(response, "User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return UnauthorizedResponse("Invalid credentials.");

            var token = JwtHelper.GenerateToken(user, _config);
            var role = user.Role;
            return Success(new { token, role }, "Login successful.");
        }
    }
}
