using AutoMapper;
using ExamApp.DTOs.Answer;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnswerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var answers = await _unitOfWork.AnswerRepo.GetAllAsync();
            return Ok(_mapper.Map<List<AnswerDto>>(answers));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var answer = await _unitOfWork.AnswerRepo.GetByIdAsync(id);
            if (answer == null) return NotFound();
            return Ok(_mapper.Map<AnswerDto>(answer));
        }

        [HttpGet("by-result/{resultId}")]
        public async Task<IActionResult> GetByResultId(int resultId)
        {
            var answers = await _unitOfWork.AnswerRepo.GetByResultIdAsync(resultId);
            return Ok(_mapper.Map<List<AnswerDto>>(answers));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAnswerDto dto)
        {
            var answer = _mapper.Map<Answer>(dto);
            await _unitOfWork.AnswerRepo.CreateAsync(answer);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = answer.Id }, _mapper.Map<AnswerDto>(answer));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateAnswerDto dto)
        {
            var existing = await _unitOfWork.AnswerRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _unitOfWork.AnswerRepo.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _unitOfWork.AnswerRepo.DeleteAsync(id);
            if (!success) return NotFound();

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
