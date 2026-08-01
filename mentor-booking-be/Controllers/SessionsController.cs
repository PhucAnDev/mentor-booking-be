using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Session;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [Protected("Student")]
        [HttpGet("student")]
        public async Task<IActionResult> ListStudentSessions()
        {
            return await _sessionService.ListStudentSessions();
        }

        [Protected("Mentor")]
        [HttpGet("mentor")]
        public async Task<IActionResult> ListMentorSessions()
        {
            return await _sessionService.ListMentorSessions();
        }

        [Protected("Mentor")]
        [HttpPost("complete/{sessionId}")]
        public async Task<IActionResult> CompleteSession(Guid sessionId)
        {
            return await _sessionService.CompleteSession(sessionId);
        }
    }
}
