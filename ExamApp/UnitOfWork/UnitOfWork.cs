using System;
using ExamApp.Models;
using ExamApp.Repositories.Implementations;
using ExamApp.Repositories.Interface;

namespace ExamApp.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IExamRepository _exams;
        private IQuestionRepository _questions;
        private IUserRepository _users;
        private IChoiceRepository _choices;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IExamRepository ExamRepo =>
            _exams ??= new ExamRepository(_context);

        public IQuestionRepository QuestionRepo =>
            _questions ??= new QuestionRepository(_context);

        public IUserRepository UserRepo=>
            _users ??= new UserRepository(_context);

        public IChoiceRepository ChoiceRepo=>
            _choices ??= new ChoiceRepository(_context);

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public int SaveChanges()=> _context.SaveChanges();
        public void Dispose() => _context.Dispose();
    }

}
