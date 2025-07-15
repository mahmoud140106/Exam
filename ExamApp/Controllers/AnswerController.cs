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
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitAnswers([FromBody] List<CreateAnswerDto> dtos)
        {
            if (dtos == null || !dtos.Any())
                return Fail("No answers provided");

            int resultId = dtos.First().ResultId ?? 0;
            if (resultId == 0)
                return Fail("Invalid result ID");

            // حفظ الإجابات
            var answers = _mapper.Map<List<Answer>>(dtos);
            foreach (var answer in answers)
            {
                await _unitOfWork.AnswerRepo.CreateAsync(answer);
            }

            await _unitOfWork.SaveChangesAsync();

            // حساب السكور
            var result = await _unitOfWork.ResultRepo.GetByIdWithAnswersAndChoicesAsync(resultId);
            if (result == null)
                return NotFoundResponse("Result not found");

            int totalAnswers = result.Answers.Count;
            int correctAnswers = result.Answers.Count(a => a.Choice != null && a.Choice.IsCorrect);

            double score = totalAnswers == 0 ? 0 : Math.Round((double)correctAnswers / totalAnswers * 100, 2);
            result.Score = score;
            result.Status = "Completed";
            result.EndTime = DateTime.UtcNow;
            result.TakenAt = DateTime.UtcNow;

            await _unitOfWork.ResultRepo.UpdateAsync(result);
            await _unitOfWork.SaveChangesAsync();

            return Success(new
            {
                message = "Exam submitted successfully",
                id = resultId,
                score = score,
                correctAnswers,
                totalAnswers
            });
        }
    }
}
