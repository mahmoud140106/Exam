using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<Question> CreateAsync(Question question);
        Task<bool> UpdateAsync(Question question);
        Task<bool> DeleteAsync(int id);
        Task<List<Question>> GetByExamIdAsync(int examId);
    }
}
