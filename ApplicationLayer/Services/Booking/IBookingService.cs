using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Booking
{
    public interface IBookingService
    {
        Task<IActionResult> CreateBooking(CreateBookingDto req);
        Task<IActionResult> ListStudentBookings();
        Task<IActionResult> ListMentorBookings();
        Task<IActionResult> ListAllBookings();
        Task<IActionResult> AcceptBooking(Guid bookingId);
        Task<IActionResult> DeclineBooking(Guid bookingId, DeclineBookingDto req);
        Task<IActionResult> CancelBooking(Guid bookingId);
        Task<IActionResult> EmergencyCancelBooking(Guid bookingId);
    }
}
