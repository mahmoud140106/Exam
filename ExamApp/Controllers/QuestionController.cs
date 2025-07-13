using AutoMapper;
using ExamApp.DTOs.Choice;
using ExamApp.DTOs.Question;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var choiceDtos = dto.ChoiceDtos;
            if (dto == null) return BadRequest("Invalid Question");
            if (choiceDtos == null || choiceDtos.Count <2) return BadRequest("Question must have at least 2 choices");

            var question = _mapper.Map<Question>(dto);
            question.Choices = _mapper.Map<List<Choice>>(choiceDtos);

            await _unitOfWork.QuestionRepo.CreateAsync(question);
            await _unitOfWork.SaveChangesAsync();

            return Success(_mapper.Map<QuestionDto>(question), "Question created successfully");
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, EditQuestionDto dto)
        {
            if (dto.Choices == null || dto.Choices.Count == 0) return BadRequest("Question must have at least two choices");
            var existing = await _unitOfWork.QuestionRepo.GetByIdWithChoicesAsync(id);
            if (existing == null)
                return NotFoundResponse("Question not found");


            // 1. Update question text
            existing.Text = dto.Text;

            // 2. Map incoming choices by ID
            var incomingChoices = dto.Choices ?? new List<ChoiceDto>();
            var incomingChoiceIds = incomingChoices
                .Where(c => c.Id != 0)
                .Select(c => c.Id)
                .ToHashSet();

            // 3. Delete missing choices (present in DB but missing in incoming)
            var toDelete = existing?.Choices?
                .Where(c => !incomingChoiceIds.Contains(c.Id))
                .ToList();

            foreach (var del in toDelete)
            {
                _unitOfWork.ChoiceRepo.Delete(del);
            }

            // 4. Add or update
            foreach (var choiceDto in incomingChoices)
            {
                if (choiceDto.Id == 0)
                {
                    // New choice
                    existing?.Choices?.Add(new Choice
                    {
                        Text = choiceDto.Text,
                        IsCorrect = choiceDto.IsCorrect,
                        QuestionId = existing.Id
                    });
                }
                else
                {
                    // Update existing
                    var existingChoice = existing?.Choices?.FirstOrDefault(c => c.Id == choiceDto.Id);
                    if (existingChoice != null)
                    {
                        _mapper.Map(choiceDto, existingChoice);
                    }
                }
            }


            //_mapper.Map(dto, existing);
            var success = await _unitOfWork.QuestionRepo.UpdateAsync(existing);
            if (!success)
                return Fail("Failed to update question");

            await _unitOfWork.SaveChangesAsync();
            return Success("Question updated successfully");
        }



        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
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
