using AutoMapper;
using ExamApp.DTOs.Choice;
using ExamApp.DTOs.Question;
using ExamApp.Models;
using ExamApp.Repositories.Interface;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoiceController : BaseApiController
    {
        public ChoiceController(IUnitOfWork _uow, IMapper _mapper)
        {
            Uow = _uow;
            Mapper = _mapper;
        }
        IUnitOfWork Uow { get; }
        IMapper Mapper { get; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var choices = await Uow.ChoiceRepo.GetAllAsync();
            //return Ok(Mapper.Map<List<ChoiceDto>>(choices));
            return Success(Mapper.Map<List<ChoiceDto>>(choices));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Console.WriteLine($"\n**************************************************\nFetching Choice with Id {id}\n**************************************************");
            var choice = await Uow.ChoiceRepo.GetByIdAsync(id);
            Console.WriteLine($"Correctly retrived {choice?.Id} {choice?.Text}");
            if (choice == null)
                return NotFoundResponse("Choice not found");
            //return NotFound();

            //return Ok(choice);
            return Success(Mapper.Map<ChoiceDto>(choice));
        }

        [HttpGet("q/{id}")]
        public async Task<IActionResult> GetByQuestionId(int id)
        {
            var choices = await Uow.ChoiceRepo.GetByQuestionIdAsync(id);
            if (choices.Count == 0)
                return NotFoundResponse("No choices found for this question");
            //return NotFound();

            return Success(Mapper.Map<List<ChoiceDto>>(choices));
            //return Ok(choices);
        }

        [HttpPost("{questionId}")]
        public async Task<IActionResult> Create(int questionId, List<CreateChoiceDto> choiceDtos)
        {
            await Uow.ChoiceRepo.AddRangeAsync(Mapper.Map<List<Choice>>(choiceDtos));
            var succeeded = await Uow.SaveChangesAsync() > 0;
            if (succeeded)
                return Success(choiceDtos, "Choices created successfully");
            //return CreatedAtAction(nameof(GetByQuestionId), new {id=questionId},choiceDtos);
            return Fail("Failed to create choices");
            //return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(ChoiceDto choiceDto, int id)
        {
            if (choiceDto.Id != id) return BadRequest();

            var exists = await Uow.ChoiceRepo.GetByIdAsync(id) != null;
            if (!exists) return NotFound();

            var choice = Mapper.Map<Choice>(choiceDto);
            await Uow.ChoiceRepo.UpdateAsync(choice);

            try
            {
                await Uow.SaveChangesAsync();
                //return NoContent();
                return Success(choiceDto, "Choice updated successfully");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("A concurrency conflict occurred.");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRange(List<ChoiceDto> choiceDtos)
        {
            if (choiceDtos == null || choiceDtos.Count == 0) return BadRequest();

            var choices = Mapper.Map<List<Choice>>(choiceDtos);
            Uow.ChoiceRepo.UpdateRange(choices);

            try
            {
                var noRowsUpdated = await Uow.SaveChangesAsync();
                if (noRowsUpdated > 0)
                    return Success(choiceDtos, "Choices updated successfully");

                //return NoContent();

                //return NotFound();
                return NotFoundResponse("No matching choices were found to update");

            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("A concurrency conflict occurred.");
            }
        }

        [HttpDelete("q/{questionId}")]
        public async Task<IActionResult> DeleteAllByQuestionIdAsync(int questionId)
        {
            var noRowsDeleted = await Uow.ChoiceRepo.DeleteAllByQuestionIdAsync(questionId);
            if (noRowsDeleted > 0)
                //return NoContent();
                return Success($"Deleted {noRowsDeleted} choices", "Choices deleted successfully");
            //return NotFound();
            return NotFoundResponse("No choices found for this question");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var choice = await Uow.ChoiceRepo.GetByIdAsync(id);
            if (choice == null)
                return NotFoundResponse("Choice not found");
            //return NotFound();
            Uow.ChoiceRepo.Delete(choice);

            try
            {
                await Uow.SaveChangesAsync();
                //return NoContent();
                return Success($"Choice with ID {id} deleted", "Choice deleted successfully");

            }
            catch
            {
                //return Conflict();
                return Conflict("Failed to delete choice due to a conflict");
            }
        }
    }
}
