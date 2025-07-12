using AutoMapper;
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
    }
}
