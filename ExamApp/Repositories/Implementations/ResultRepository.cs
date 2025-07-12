using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class ResultRepository : IResultRepository
    {
        private readonly ApplicationDbContext _context;

        public ResultRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Result>> GetAllAsync()
        {
            return await _context.Results
                .Include(r => r.Student)
                .Include(r => r.Exam)
                .Include(r => r.Answers)
                .ToListAsync();
        }

        public async Task<Result?> GetByIdAsync(int id)
        {
            return await _context.Results
                .Include(r => r.Student)
                .Include(r => r.Exam)
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<Result> CreateAsync(Result result)
        {
            _context.Results.Add(result);
            return Task.FromResult(result);
        }

        public Task<bool> UpdateAsync(Result result)
        {
            _context.Results.Update(result);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await GetByIdAsync(id);
            if (result == null) return false;
            _context.Results.Remove(result);
            return true;
        }

        public async Task<List<Result>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Results
                .Where(r => r.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<List<Result>> GetByExamIdAsync(int examId)
        {
            return await _context.Results
                .Where(r => r.ExamId == examId)
                .ToListAsync();
        }
    }
}
