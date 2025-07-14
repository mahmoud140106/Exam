using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IExamRepository : IRepository<Exam>
    {
        Task<List<Exam>> GetAll(string? name, string? sortBy, bool isDesc, int page, int pageSize);
        Task<int> CountAsync(string? name);
    }
}
