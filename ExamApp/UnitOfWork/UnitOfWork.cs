using System;
using AutoMapper;
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
        private IAnswerRepository _answers;
        private IResultRepository _results;


        public UnitOfWork(ApplicationDbContext context, IMapper _mapper)
        {
            _context = context;
            Mapper = _mapper;
        }

        public IExamRepository ExamRepo =>
            _exams ??= new ExamRepository(_context);

        public IQuestionRepository QuestionRepo =>
            _questions ??= new QuestionRepository(_context);

        public IAnswerRepository AnswerRepo =>
          _answers ??= new AnswerRepository(_context);
        public IUserRepository UserRepo=>
            _users ??= new UserRepository(_context);

        public IResultRepository ResultRepo =>
            _results ??= new ResultRepository(_context,Mapper);

        public IChoiceRepository ChoiceRepo=>
            _choices ??= new ChoiceRepository(_context);

        public IMapper Mapper { get; }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

        public int SaveChanges()=> _context.SaveChanges();
        public void Dispose() => _context.Dispose();
    }

}
