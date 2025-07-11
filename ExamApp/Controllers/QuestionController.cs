using AutoMapper;
using ExamApp.DTOs.Question;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _unitOfWork.QuestionRepo.GetAllAsync();
            return Ok(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _unitOfWork.QuestionRepo.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(_mapper.Map<QuestionDto>(question));
        }

        [HttpGet("by-exam/{examId}")]
        public async Task<IActionResult> GetByExamId(int examId)
        {
            var questions = await _unitOfWork.QuestionRepo.GetByExamIdAsync(examId);
            return Ok(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            await _unitOfWork.QuestionRepo.CreateAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = question.Id }, _mapper.Map<QuestionDto>(question));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateQuestionDto dto)
        {
            var existing = await _unitOfWork.QuestionRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var success = await _unitOfWork.QuestionRepo.UpdateAsync(existing);
            if (!success) return StatusCode(500);

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _unitOfWork.QuestionRepo.DeleteAsync(id);
            if (!deleted) return NotFound();

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
