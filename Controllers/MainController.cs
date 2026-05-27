using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebWerverPart.Models.DTO;
using WebWerverPart.Services;

namespace WebWerverPart.Controllers
{
    [Route("api/main")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly MainService _mainService;

        public MainController(MainService mainService)
        {
            _mainService = mainService;
        }

        [Authorize]
        [HttpPost("bug_report")]
        public async Task<IActionResult> BugReport([FromBody] ReportDTO reportDTO)
        {
            var userId = User.FindFirst("userId")?.Value;

            if (userId == null)
                return Unauthorized();

            if (!int.TryParse(userId, out var parsedUserId))
                return Unauthorized();

            var (success, Emessage) = await _mainService.SendBugReportAsync(reportDTO, parsedUserId);
            if (!success)
            {
                if (Emessage != "another server error")
                    return BadRequest(new { message = Emessage });

                return StatusCode(500, new { message = Emessage });
            }

            return Ok(new { success = true });
        }
    }
}
