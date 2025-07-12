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
    public class QuestionController : BaseApiController
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
            return Success(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _unitOfWork.QuestionRepo.GetByIdAsync(id);
            if (question == null)
                return NotFoundResponse("Question not found");

            return Success(_mapper.Map<QuestionDto>(question));
        }

        [HttpGet("by-exam/{examId}")]
        public async Task<IActionResult> GetByExamId(int examId)
        {
            var questions = await _unitOfWork.QuestionRepo.GetByExamIdAsync(examId);
            if (questions == null || questions.Count == 0)
                return NotFoundResponse("No questions found for this exam");

            return Success(_mapper.Map<List<QuestionDto>>(questions));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var question = _mapper.Map<Question>(dto);
            await _unitOfWork.QuestionRepo.CreateAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return Success(_mapper.Map<QuestionDto>(question), "Question created successfully");
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CreateQuestionDto dto)
        {
            var existing = await _unitOfWork.QuestionRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFoundResponse("Question not found");

            _mapper.Map(dto, existing);
            var success = await _unitOfWork.QuestionRepo.UpdateAsync(existing);
            if (!success)
                return Fail("Failed to update question");

            await _unitOfWork.SaveChangesAsync();
            return Success("Question updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _unitOfWork.QuestionRepo.DeleteAsync(id);
            if (!deleted)
                return NotFoundResponse("Question not found or already deleted");

            await _unitOfWork.SaveChangesAsync();
            return Success("Question deleted successfully");
        }
    }
}
