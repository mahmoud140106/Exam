using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IChoiceRepository:IRepository<Choice>
    {
        Task<List<Choice>> GetByQuestionIdAsync(int questionId);
        Task<bool> AddRangeAsync(params List<Choice> choices);
    }
}
