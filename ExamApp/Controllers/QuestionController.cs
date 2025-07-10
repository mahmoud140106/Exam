using AutoMapper;
using ExamApp.DTOs.Question;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _repository;
        private readonly IMapper _mapper;

        public QuestionController(IQuestionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var questions = await _repository.GetAllAsync();
            return Ok(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _repository.GetByIdAsync(id);
            if (question == null) return NotFound();
            return Ok(_mapper.Map<QuestionDto>(question));
        }

        [HttpGet("by-exam/{examId}")]
        public async Task<IActionResult> GetByExamId(int examId)
        {
            var questions = await _repository.GetByExamIdAsync(examId);
            return Ok(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            var created = await _repository.CreateAsync(question);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<QuestionDto>(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateQuestionDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            var updated = await _repository.UpdateAsync(existing);
            return updated ? NoContent() : StatusCode(500);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
