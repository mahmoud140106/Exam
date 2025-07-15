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

        public async Task<bool> AddRangeAsync(List<Choice> choices)
        {
           await Db.Choices.AddRangeAsync(choices);
           return true;
        }

        public async Task<Choice> CreateAsync(Choice entity)
        {
            var choice = await Db.Choices.AddAsync(entity);
            return choice.Entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var choice = await Db.Choices.FindAsync(id);
            if(choice == null)
                return false;
            Db.Choices.Remove(choice);
            return true;
        }
        public bool Delete(Choice choice)
        {
            if(choice == null)
                return false;
           
            Db.Choices.Remove(choice);
            return true;
        }

        //public async Task<int> DeleteAllByQuestionIdAsync(int questionId)
        //{
        //    var noRowsDeleted = await Db.Choices.Where(c => c.QuestionId == questionId && c.Answers.Count == 0)
        //              .ExecuteDeleteAsync();
        //    return noRowsDeleted;
        //}
        public async Task<int> DeleteAllByQuestionIdAsync(int questionId)
        {
            var choicesToDelete = await Db.Choices
                .Where(c => c.QuestionId == questionId && c.Answers.Count == 0)
                .ToListAsync();  

            Db.Choices.RemoveRange(choicesToDelete);
            await Db.SaveChangesAsync();

            return choicesToDelete.Count;
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

        public async Task<bool> UpdateAsync(Choice entity)
        {
            var entry = Db.Entry<Choice>(entity);

            if (entry == null) return false;

            entry.State = EntityState.Modified;
            return true;
        }
        public bool UpdateRange(List<Choice> choices)
        {
            if(choices == null || choices.Count==0) return false;
            Db.Choices.UpdateRange(choices);
            return true;
        }
    }
}
