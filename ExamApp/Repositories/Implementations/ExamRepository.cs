﻿using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Repositories.Implementations
{
    public class ExamRepository : IExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Exam>> GetAllAsync()
        {
            return await _context.Exams.ToListAsync();
        }

        public async Task<List<Exam>> GetAll(
     string? name,
     string? sortBy,
     bool isDesc,
     int page,
     int pageSize,
     bool? isActive = null)
        {
            var now = DateTime.UtcNow;
            var query = _context.Exams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.Title.ToLower().Contains(name.ToLower()));
            }

            if (isActive == true)
            {
                query = query.Where(e => e.IsActive==true);
            }

            query = sortBy switch
            {
                "title" => isDesc ? query.OrderByDescending(e => e.Title) : query.OrderBy(e => e.Title),
                "createdAt" => isDesc ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt),
                _ => isDesc ? query.OrderByDescending(e => e.Id) : query.OrderBy(e => e.Id),
            };

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var exams = await query.ToListAsync();

            return exams;
        }


        public async Task<int> CountAsync(string? name, bool isActive)
        {
            var now = DateTime.UtcNow;
            var query = _context.Exams.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.Title.ToLower().Contains(name.ToLower()));
            }

            if (isActive)
            {
                query = query.Where(e => e.StartTime <= now && e.EndTime >= now);
            }

            return await query.CountAsync();
        }



        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams.FindAsync(id);
        }

        public Task<Exam> CreateAsync(Exam exam)
        {
            _context.Exams.Add(exam);
            return Task.FromResult(exam);
        }

        public Task<bool> UpdateAsync(Exam exam)
        {
            _context.Exams.Update(exam);
            return Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return false;
            _context.Exams.Remove(exam);
            return true;
        }
    }
}
