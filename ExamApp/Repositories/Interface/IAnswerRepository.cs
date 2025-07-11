using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<List<Answer>> GetByResultIdAsync(int resultId);
        Task<List<Answer>> GetByQuestionIdAsync(int questionId);
    }
}
