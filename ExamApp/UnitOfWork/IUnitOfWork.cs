using ExamApp.Models;
using ExamApp.Repositories.Implementations;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.UnitOfWork
{
    public interface IUnitOfWork:IDisposable
    {
        IExamRepository ExamRepo { get; }

        IQuestionRepository QuestionRepo { get; }

        IAnswerRepository AnswerRepo { get; }
        IUserRepository UserRepo { get; }
        IChoiceRepository ChoiceRepo { get; }

        IResultRepository ResultRepo { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
