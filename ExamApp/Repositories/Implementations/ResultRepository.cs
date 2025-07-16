﻿using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamApp.DTOs.Answer;
using ExamApp.DTOs.Question;
using ExamApp.DTOs.Result;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class ResultRepository : IResultRepository
    {
        private readonly ApplicationDbContext _context;

        private IMapper Mapper { get; }

        public ResultRepository(ApplicationDbContext context, IMapper _mapper)
        {
            _context = context;
            Mapper = _mapper;
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
        public async Task<Result?> GetByIdWithAnswersAndChoicesAsync(int resultId)
        {
            return await _context.Results
                .Include(r => r.Answers)
                    .ThenInclude(a => a.Choice)
                .FirstOrDefaultAsync(r => r.Id == resultId);
        }

        public async Task<List<StudentResultDTO>> GetByStudentIdWithDetailsAsync(int studentId)
        {
            var results = await _context.Results.Where(r => r.StudentId == studentId).ProjectTo<StudentResultDTO>(Mapper.ConfigurationProvider).ToListAsync();
            return results;
        }


        public List<Answer> GetPage(int resultId,int page = 1, int pageSize = 10)
        {
            Console.WriteLine("********************************************************************\nBegin");
            var answer = _context.Answers.AsNoTracking().Where(a=> a.ResultId == resultId);

            answer= answer.Include(a => a.Question).ThenInclude(q => q.Choices);
            //int courseCount = answer.Count();

            answer = answer.Skip((page - 1) * pageSize)
                            .Take(pageSize);

            return answer.ToList();
        }
    }
}
