using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Student;
using ApplicationLayer.Services.Mentor;
using ApplicationLayer.Services.Booking;
using ApplicationLayer.Services.Session;
using ApplicationLayer.Services.Minutes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/admin")]
    [Protected("Admin")] // Enforce admin check for all endpoints in this controller
    public class AdminController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMentorService _mentorService;
        private readonly IBookingService _bookingService;
        private readonly ISessionService _sessionService;
        private readonly IMeetingMinutesService _minutesService;

        public AdminController(
            IStudentService studentService,
            IMentorService mentorService,
            IBookingService bookingService,
            ISessionService sessionService,
            IMeetingMinutesService minutesService)
        {
            _studentService = studentService;
            _mentorService = mentorService;
            _bookingService = bookingService;
            _sessionService = sessionService;
            _minutesService = minutesService;
        }

        [HttpGet("students")]
        public async Task<IActionResult> ListStudents()
        {
            return await _studentService.ListStudents();
        }

        [HttpPost("strikes/{studentId}")]
        public async Task<IActionResult> UpdateStrikes(Guid studentId, [FromQuery] int count)
        {
            return await _studentService.UpdateStrikes(studentId, count);
        }

        [HttpGet("mentors")]
        public async Task<IActionResult> ListMentors()
        {
            return await _mentorService.ListAllMentors();
        }

        [HttpPost("mentors/toggle/{mentorId}")]
        public async Task<IActionResult> ToggleMentor(Guid mentorId)
        {
            return await _mentorService.ToggleActivationStatus(mentorId);
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> ListBookings()
        {
            return await _bookingService.ListAllBookings();
        }

        [HttpPost("bookings/cancel/{bookingId}")]
        public async Task<IActionResult> EmergencyCancel(Guid bookingId)
        {
            return await _bookingService.EmergencyCancelBooking(bookingId);
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> ListSessions()
        {
            return await _sessionService.ListAllSessions();
        }

        [HttpGet("minutes")]
        public async Task<IActionResult> ListMinutes()
        {
            return await _minutesService.ListAllMinutes();
        }
    }
}
