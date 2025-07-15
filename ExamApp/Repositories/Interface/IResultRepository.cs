using ExamApp.DTOs.Result;
using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IResultRepository : IRepository<Result>
    {
        Task<List<Result>> GetByStudentIdAsync(int studentId);
        Task<List<Result>> GetByExamIdAsync(int examId);

        Task<Result?> GetByIdWithAnswersAndChoicesAsync(int resultId);
        Task<List<StudentResultDTO>> GetByStudentIdWithDetailsAsync(int studentId);
    }
}
