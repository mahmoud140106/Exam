using AutoMapper;
using ExamApp.DTOs.Exam;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   
    public class ExamController : ControllerBase
    {
        private readonly IExamRepository _repository;
        private readonly IMapper _mapper;

        public ExamController(IExamRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _repository.GetAllAsync();
            return Ok(_mapper.Map<List<ExamDto>>(exams));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exam = await _repository.GetByIdAsync(id);
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
            var created = await _repository.CreateAsync(exam);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<ExamDto>(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateExamDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var success = await _repository.UpdateAsync(existing);
            return success ? NoContent() : StatusCode(500);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _repository.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
