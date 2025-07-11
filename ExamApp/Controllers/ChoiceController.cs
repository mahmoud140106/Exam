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
    public class ChoiceController : ControllerBase
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
            return Ok(Mapper.Map<List<ChoiceDto>>(choices));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Console.WriteLine($"\n**************************************************\nFetching Choice with Id {id}\n**************************************************");
            var choice = await Uow.ChoiceRepo.GetByIdAsync(id);
            Console.WriteLine($"Correctly retrived {choice?.Id} {choice?.Text}");
            if(choice == null) return NotFound();

            return Ok(choice);
        }

        [HttpGet("q/{id}")]
        public async Task<IActionResult> GetByQuestionId(int id)
        {
            var choices = await Uow.ChoiceRepo.GetByQuestionIdAsync(id);
            if(choices.Count == 0) return NotFound();

            return Ok(choices);
        }

        [HttpPost("{questionId}")]
        public async Task<IActionResult> Create(int questionId,List<ChoiceDto> choiceDtos)
        {
            await Uow.ChoiceRepo.AddRangeAsync(Mapper.Map<List<Choice>>(choiceDtos));
            var succeeded = await Uow.SaveChangesAsync() >0;
            if (succeeded) return CreatedAtAction(nameof(GetByQuestionId), new {id=questionId},choiceDtos);
            return BadRequest();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(ChoiceDto choiceDto, int id)
        {
            
            if(choiceDto.Id != id) return BadRequest();

            var exists= await Uow.ChoiceRepo.GetByIdAsync(id) !=null;
            if(!exists) return NotFound();

            var choice = Mapper.Map<Choice>(choiceDto);
            await Uow.ChoiceRepo.UpdateAsync(choice);

            try
            {
                await Uow.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("A concurrency conflict occurred.");
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateRange(List<ChoiceDto> choiceDtos)
        {
            
            if(choiceDtos ==null || choiceDtos.Count ==0) return BadRequest();


            var choices = Mapper.Map<List<Choice>>(choiceDtos);
            Uow.ChoiceRepo.UpdateRange(choices);

            try
            {
                var noRowsUpdated = await Uow.SaveChangesAsync();
                if (noRowsUpdated > 0) return NoContent();
                    
                return NotFound();
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
                return NoContent();
            return NotFound();
        }
        
        [HttpDelete("{id}")]    
        public async Task<IActionResult> DeleteByIdAsync(int id)
        {
            var choice = await Uow.ChoiceRepo.GetByIdAsync(id);
            if (choice == null) return NotFound();
            Uow.ChoiceRepo.Delete(choice);

            try
            {
                await Uow.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return Conflict();
            }
        }


    }

}
