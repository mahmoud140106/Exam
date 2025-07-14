using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<User> RegisterAsync(User user);
        Task<(List<User> users, int totalCount)> GetPagedFilteredUsersAsync(string? search, int page, int pageSize);
        Task<bool> HasRelatedResultsAsync(int userId);
        void Update(User user);
        void Delete(User user);

        Task<List<User>> GetAll(string? name, string sortBy, bool isDesc, int page, int pageSize, bool? isDeleted);
        Task<int> CountAsync(string? name, bool? isDeleted);

    }
}
