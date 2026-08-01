using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Booking;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Protected("Student")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto req)
        {
            return await _bookingService.CreateBooking(req);
        }

        [Protected("Student")]
        [HttpGet("student")]
        public async Task<IActionResult> ListStudentBookings()
        {
            return await _bookingService.ListStudentBookings();
        }

        [Protected("Mentor")]
        [HttpGet("mentor")]
        public async Task<IActionResult> ListMentorBookings()
        {
            return await _bookingService.ListMentorBookings();
        }

        [Protected("Mentor")]
        [HttpPost("accept/{bookingId}")]
        public async Task<IActionResult> AcceptBooking(Guid bookingId)
        {
            return await _bookingService.AcceptBooking(bookingId);
        }

        [Protected("Mentor")]
        [HttpPost("decline/{bookingId}")]
        public async Task<IActionResult> DeclineBooking(Guid bookingId, [FromBody] DeclineBookingDto req)
        {
            return await _bookingService.DeclineBooking(bookingId, req);
        }

        [Protected("Student")]
        [HttpPost("cancel/{bookingId}")]
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            return await _bookingService.CancelBooking(bookingId);
        }
    }
}
