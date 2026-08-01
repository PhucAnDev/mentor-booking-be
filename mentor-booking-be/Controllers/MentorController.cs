using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Mentor;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/mentors")]
    public class MentorController : ControllerBase
    {
        private readonly IMentorService _mentorService;

        public MentorController(IMentorService mentorService)
        {
            _mentorService = mentorService;
        }

        [Protected("Mentor")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return await _mentorService.GetProfile();
        }

        [Protected("Mentor")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateMentorProfileDto req)
        {
            return await _mentorService.UpdateProfile(req);
        }

        [HttpGet("active")]
        public async Task<IActionResult> ListActiveMentors()
        {
            return await _mentorService.ListActiveMentors();
        }

        [HttpGet("slots/{mentorId}")]
        public async Task<IActionResult> GetSlots(Guid mentorId)
        {
            return await _mentorService.GetSlots(mentorId);
        }

        [Protected("Mentor")]
        [HttpPost("slots")]
        public async Task<IActionResult> UpdateSlots([FromBody] List<CreateSlotDto> req)
        {
            return await _mentorService.UpdateSlots(req);
        }
    }
}
