using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Answer>> GetAllAsync()
        {
            return await _context.Answers
                .Include(a => a.Choice)
                .Include(a => a.Question)
                .Include(a => a.Result)
                .ToListAsync();
        }

        public async Task<Answer?> GetByIdAsync(int id)
        {
            return await _context.Answers
                .Include(a => a.Choice)
                .Include(a => a.Question)
                .Include(a => a.Result)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<Answer> CreateAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            return Task.FromResult(answer); // Save in controller
        }

        public Task<bool> UpdateAsync(Answer answer)
        {
            _context.Answers.Update(answer);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var answer = await GetByIdAsync(id);
            if (answer == null) return false;
            _context.Answers.Remove(answer);
            return true;
        }

        public async Task<List<Answer>> GetByResultIdAsync(int resultId)
        {
            return await _context.Answers
                .Where(a => a.ResultId == resultId)
                .ToListAsync();
        }

        public async Task<List<Answer>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}
