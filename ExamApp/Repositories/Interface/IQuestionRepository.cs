using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IQuestionRepository:IRepository<Question>
    {
        Task<List<Question>> GetByExamIdAsync(int examId);
    }
}
