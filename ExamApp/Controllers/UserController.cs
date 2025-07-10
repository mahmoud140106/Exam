using AutoMapper;
using ExamApp.DTOs.User;
using ExamApp.Helpers;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using ExamApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserController(IUserRepository userRepository, IMapper mapper, IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _userRepository.IsEmailExistsAsync(dto.Email))
                return BadRequest("Email already exists.");

            if (await _userRepository.IsUsernameExistsAsync(dto.Username))
                return BadRequest("Username already exists.");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Role = "Student"; // Default role

            var createdUser = await _userRepository.RegisterAsync(user);
            var response = _mapper.Map<UserResponseDto>(createdUser);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials.");

            var token = JwtHelper.GenerateToken(user, _config);
            return Ok(new { token });
        }
    }
}
