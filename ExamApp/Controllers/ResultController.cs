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
    public class ResultController : ControllerBase
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
            return Ok(_mapper.Map<List<ResultDto>>(results));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _unitOfWork.ResultRepo.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(_mapper.Map<ResultDto>(result));
        }

        [HttpGet("by-student/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var results = await _unitOfWork.ResultRepo.GetByStudentIdAsync(studentId);
            return Ok(_mapper.Map<List<ResultDto>>(results));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateResultDto dto)
        {
            var result = _mapper.Map<Result>(dto);
            await _unitOfWork.ResultRepo.CreateAsync(result);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, _mapper.Map<ResultDto>(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateResultDto dto)
        {
            var existing = await _unitOfWork.ResultRepo.GetByIdAsync(id);
            if (existing == null) return NotFound();

            _mapper.Map(dto, existing);
            await _unitOfWork.ResultRepo.UpdateAsync(existing);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _unitOfWork.ResultRepo.DeleteAsync(id);
            if (!success) return NotFound();

            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }
}
