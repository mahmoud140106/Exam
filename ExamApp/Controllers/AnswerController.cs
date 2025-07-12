using AutoMapper;
using ExamApp.Controllers;
using ExamApp.DTOs.Answer;
using ExamApp.Models;
using ExamApp.Responses;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : BaseApiController
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
            var dtos = _mapper.Map<List<AnswerDto>>(answers);
            return Success(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var answer = await _unitOfWork.AnswerRepo.GetByIdAsync(id);
            if (answer == null)
                return NotFoundResponse("Answer not found");

            var dto = _mapper.Map<AnswerDto>(answer);
            return Success(dto);
        }

        [HttpGet("by-result/{resultId}")]
        public async Task<IActionResult> GetByResultId(int resultId)
        {
            var answers = await _unitOfWork.AnswerRepo.GetByResultIdAsync(resultId);
            var dtos = _mapper.Map<List<AnswerDto>>(answers);
            return Success(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAnswerDto dto)
        {
            var answer = _mapper.Map<Answer>(dto);
            await _unitOfWork.AnswerRepo.CreateAsync(answer);
            await _unitOfWork.SaveChangesAsync();

            var createdDto = _mapper.Map<AnswerDto>(answer);
            return Success(createdDto, "Answer created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateAnswerDto dto)
        {
            var existing = await _unitOfWork.AnswerRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFoundResponse("Answer not found");

            _mapper.Map(dto, existing);
            await _unitOfWork.AnswerRepo.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return Success<object>(null, "Answer updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _unitOfWork.AnswerRepo.DeleteAsync(id);
            if (!success)
                return NotFoundResponse("Answer not found");

            await _unitOfWork.SaveChangesAsync();
            return Success<object>(null, "Answer deleted successfully");
        }
    }
}
