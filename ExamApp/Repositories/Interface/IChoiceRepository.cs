using ExamApp.Models;

namespace ExamApp.Repositories.Interface
{
    public interface IChoiceRepository:IRepository<Choice>
    {
        bool UpdateRange(List<Choice> choices);
        Task<List<Choice>> GetByQuestionIdAsync(int questionId);
        Task<bool> AddRangeAsync(params List<Choice> choices);
        Task<int> DeleteAllByQuestionIdAsync(int questionId);
        bool Delete(Choice choice);
    }
}
