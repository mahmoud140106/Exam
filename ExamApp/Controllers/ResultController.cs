using AutoMapper;
using ExamApp.DTOs.Answer;
using ExamApp.DTOs.Result;
using ExamApp.Models;
using ExamApp.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ResultController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var results = await _unitOfWork.ResultRepo.GetAllAsync();
            return Success(_mapper.Map<List<ResultDto>>(results));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.ResultRepo.GetByIdAsync(id);
            if (result == null)
                return NotFoundResponse("Result not found");

            return Success(_mapper.Map<ResultDto>(result));
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var results = await _unitOfWork.ResultRepo.GetByStudentIdAsync(studentId);
            if (results == null || results.Count == 0)
                return NotFoundResponse("No results found for this student");

            return Success(_mapper.Map<List<ResultDto>>(results));
        }
        [HttpGet("by-student/detailed/{studentId}")]
        public async Task<IActionResult> GetByStudentIdWithDetails(int studentId)
        {
            if (studentId == 0) return NotFoundResponse();
            var student = await _unitOfWork.UserRepo.GetByIdAsync(studentId);
            if(student == null) return NotFoundResponse();

            var results = await _unitOfWork.ResultRepo.GetByStudentIdWithDetailsAsync(studentId);
            return Success(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateResultDto dto)
        {
            var result = _mapper.Map<Result>(dto);
            await _unitOfWork.ResultRepo.CreateAsync(result);
            await _unitOfWork.SaveChangesAsync();

            return Success(_mapper.Map<ResultDto>(result), "Result created successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateResultDto dto)
        {
            var existing = await _unitOfWork.ResultRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFoundResponse("Result not found");

            _mapper.Map(dto, existing);
            var updated = await _unitOfWork.ResultRepo.UpdateAsync(existing);
            if (!updated)
                return Fail("Failed to update result");

            await _unitOfWork.SaveChangesAsync();
            return Success("Result updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _unitOfWork.ResultRepo.DeleteAsync(id);
            if (!deleted)
                return NotFoundResponse("Result not found");

            await _unitOfWork.SaveChangesAsync();
            return Success("Result deleted successfully");
        }
        [HttpGet("page/{id}")]
        public async Task<IActionResult>GetAnswerWithDetails(int id,[FromQuery] int page=1,[FromQuery] int pageSize=10)
        {
            if (id == 0) return BadRequest();

            var result = await _unitOfWork.ResultRepo.GetByIdAsync(id);
            
            if (result == null) return NotFoundResponse();
            if(page<1) page = 1;

            var pageData= _unitOfWork.ResultRepo.GetPage(id, page,pageSize);
            if (pageData == null || pageData.Count == 0) return NotFoundResponse();
            //var mapped = _mapper.Map<List<AnswerWithQuestions>>(pageData);
            return Success(pageData);
        }
    }
}
