using ExamApp.Responses;
using Microsoft.AspNetCore.Mvc;

namespace ExamApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "")
        {
            return Ok(ApiResponse<T>.Success(data, message));
        }

        protected IActionResult Fail(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, ApiResponse<string>.Fail(message));
        }

        protected IActionResult NotFoundResponse(string message = "Not found")
        {
            return NotFound(ApiResponse<string>.Fail(message));
        }

        protected IActionResult InternalError(string message = "Internal server error")
        {
            return StatusCode(500, ApiResponse<string>.Fail(message));
        }

        protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
        {
            return Unauthorized(ApiResponse<string>.Fail(message));
        }
    }
}
