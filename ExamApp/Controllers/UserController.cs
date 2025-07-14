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



        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var (users, total) = await _unitOfWork.UserRepo.GetPagedFilteredUsersAsync(search, page, pageSize);
        //    var response = _mapper.Map<List<UserDto>>(users);
        //    return Success(new { data = response, total }, "Users fetched successfully.");
        //}


        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
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

            var response = _mapper.Map<UserDto>(user);
            return Success(response, "Student created successfully.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserUpdateDto dto)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);
            if (user == null) return NotFoundResponse();

            if (!string.IsNullOrEmpty(dto.Username)) user.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _unitOfWork.UserRepo.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Success<string?>(null, "Student updated successfully.");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _unitOfWork.UserRepo.GetByIdAsync(id);
            if (user == null) return NotFoundResponse();

            //if (await _unitOfWork.UserRepo.HasRelatedResultsAsync(id))
            //    return Fail("Cannot delete student with existing results.");

            _unitOfWork.UserRepo.Delete(user);
            await _unitOfWork.SaveChangesAsync();

            return Success<object?>(null, "Student deleted successfully.");

        }



        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] string? sortBy = "id",
        [FromQuery] bool isDesc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isDeleted = null)
        {
            var users = await _unitOfWork.UserRepo.GetAll(name, sortBy, isDesc, page, pageSize, isDeleted);
            var totalCount = await _unitOfWork.UserRepo.CountAsync(name, isDeleted);

            var result = new
            {
                totalCount,
                page,
                pageSize,
                data = _mapper.Map<List<UserDto>>(users)
            };

            return Success(result);
        }

    }
}
