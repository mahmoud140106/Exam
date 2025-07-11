using AutoMapper;
using ExamApp.DTOs.Exam;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _unitOfWork.ExamRepo.GetAllAsync();
            return Ok(_mapper.Map<List<ExamDto>>(exams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exam = await _unitOfWork.ExamRepo.GetByIdAsync(id);
            if (exam == null) return NotFound();
            return Ok(_mapper.Map<ExamDto>(exam));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateExamDto dto)
        {
            var exam = _mapper.Map<Exam>(dto);
            exam.CreatedAt = DateTime.Now;
            exam.IsActive = true;

            await _unitOfWork.ExamRepo.CreateAsync(exam);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = exam.Id }, _mapper.Map<ExamDto>(exam));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateExamDto dto)
        {
            var existing = await _unitOfWork.ExamRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var success = await _unitOfWork.ExamRepo.UpdateAsync(existing);
            if (!success) return StatusCode(500);

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _unitOfWork.ExamRepo.DeleteAsync(id);
            if (!success) return NotFound();

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
