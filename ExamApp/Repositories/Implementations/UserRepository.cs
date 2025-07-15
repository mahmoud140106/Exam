using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.Where(u => !u.IsDeleted &&u.Role != "Admin").ToListAsync();
        }
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username && !u.IsDeleted);
        }

        public Task<User> RegisterAsync(User user)
        {
            _context.Users.Add(user);
            return Task.FromResult(user);
        }



        public async Task<(List<User>, int)> GetPagedFilteredUsersAsync(string? search, int page, int pageSize)
        {
            var query = _context.Users.Where(u => !u.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Username.Contains(search) || u.Email.Contains(search));
            }

            var total = await query.CountAsync();

            var result = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (result, total);
        }

        public async Task<bool> HasRelatedResultsAsync(int userId)
        {
            return await _context.Results.AnyAsync(r => r.StudentId == userId);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            //_context.Users.Remove(user);
            user.IsDeleted = true;
            _context.Users.Update(user);
        }


        public async Task<List<User>> GetAll(string? name, string sortBy, bool isDesc, int page, int pageSize, bool? isDeleted)
        {
            var query = _context.Users.Where(u => u.Role != "Admin").AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.Username.Contains(name));

            if (isDeleted != null)
                query = query.Where(u => u.IsDeleted == isDeleted.Value);

            // Sorting
            query = sortBy.ToLower() switch
            {
                "username" => isDesc ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                _ => isDesc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
            };

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<int> CountAsync(string? name, bool? isDeleted)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => u.Username.Contains(name));

            if (isDeleted != null)
                query = query.Where(u => u.IsDeleted == isDeleted.Value);

            return await query.CountAsync();
        }
    }
}
