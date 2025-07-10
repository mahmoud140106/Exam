using System.Linq.Expressions;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class ChoiceRepository:IChoiceRepository
    {
        public ChoiceRepository(ApplicationDbContext _db)
        {
            Db = _db;
        }

        public ApplicationDbContext Db { get; }

        public async Task<bool> AddRangeAsync(params List<Choice> choices)
        {
           await Db.Choices.AddRangeAsync(choices);
           return true;
        }

        public async Task<Choice> CreateAsync(Choice entity)
        {
            var choice = await Db.Choices.AddAsync(entity);
            return choice.Entity;
        }

        public Task<bool> DeleteAsync(int id)
        {
            var choice = Db.Choices.Find(id);
            if(choice == null)
                return Task.FromResult(false);
           
            Db.Choices.Remove(choice);
            return Task.FromResult(true);
        }

        public async Task<List<Choice>> GetAllAsync()
        {
            return await Db.Choices.AsNoTracking().ToListAsync();
        }

        public async Task<Choice?> GetByIdAsync(int id)
        {
            return await Db.Choices.FindAsync(id);
        }

        public async Task<List<Choice>> GetByQuestionIdAsync(int questionId)
        {
            return await Db.Choices.Where(c=> c.QuestionId == questionId).ToListAsync();
        }

        public Task<bool> UpdateAsync(Choice entity)
        {
            var entry = Db.Entry<Choice>(entity);

            if (entry == null) return new Task<bool>(()=> false);

            entry.State = EntityState.Modified;
            return new Task<bool>(() => true);
        }
    }
}
