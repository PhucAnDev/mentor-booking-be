using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Minutes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/minutes")]
    public class MeetingMinutesController : ControllerBase
    {
        private readonly IMeetingMinutesService _minutesService;

        public MeetingMinutesController(IMeetingMinutesService minutesService)
        {
            _minutesService = minutesService;
        }

        [Protected("Student")]
        [HttpPost("student/{sessionId}")]
        public async Task<IActionResult> SubmitMinutesByStudent(Guid sessionId, [FromBody] SubmitMinutesByStudentDto req)
        {
            return await _minutesService.SubmitMinutesByStudent(sessionId, req);
        }

        [Protected("Mentor")]
        [HttpPost("mentor/{sessionId}")]
        public async Task<IActionResult> SubmitMinutesByMentor(Guid sessionId, [FromBody] SubmitMinutesByMentorDto req)
        {
            return await _minutesService.SubmitMinutesByMentor(sessionId, req);
        }

        [Protected]
        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetMinutesBySessionId(Guid sessionId)
        {
            return await _minutesService.GetMinutesBySessionId(sessionId);
        }
    }
}
