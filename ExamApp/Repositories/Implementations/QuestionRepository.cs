using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetAllAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            return await _context.Questions.FindAsync(id);
        }

        public Task<Question> CreateAsync(Question question)
        {
            _context.Questions.Add(question);
            return Task.FromResult(question);
        }

        public Task<bool> UpdateAsync(Question question)
        {
            _context.Questions.Update(question);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var question = await GetByIdAsync(id);
            if (question == null) return false;
            _context.Questions.Remove(question);
            return true; 
        }

        public async Task<List<Question>> GetByExamIdAsync(int examId)
        {
            return await _context.Questions
                .Where(q => q.ExamId == examId)
                .ToListAsync();
        }

        public async Task<Question> GetByIdWithChoicesAsync(int id)
        {
            return await _context.Questions
                        .Include(q => q.Choices)
                        .FirstOrDefaultAsync(q => q.Id == id);        
        }
    }
}
