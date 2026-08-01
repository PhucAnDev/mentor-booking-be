using ApplicationLayer.DTOs;
using ApplicationLayer.ResponseCode;
using AutoMapper;
using DomainLayer.Entities;
using InfrastructureLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DomainLayer.Enum.GeneralEnum;

namespace ApplicationLayer.Services.Booking
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IGenericRepository<Bookings> _bookingRepo;
        private readonly IGenericRepository<Students> _studentRepo;
        private readonly IGenericRepository<Mentors> _mentorRepo;
        private readonly IGenericRepository<Slots> _slotRepo;
        private readonly IGenericRepository<Sessions> _sessionRepo;

        public BookingService(
            IGenericRepository<Bookings> bookingRepo,
            IGenericRepository<Students> studentRepo,
            IGenericRepository<Mentors> mentorRepo,
            IGenericRepository<Slots> slotRepo,
            IGenericRepository<Sessions> sessionRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _bookingRepo = bookingRepo;
            _studentRepo = studentRepo;
            _mentorRepo = mentorRepo;
            _slotRepo = slotRepo;
            _sessionRepo = sessionRepo;
        }

        public async Task<IActionResult> CreateBooking(CreateBookingDto req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId, "User");
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin sinh viên.");

            // Rule 1: Check ban
            if (student.BannedUntil.HasValue && student.BannedUntil.Value > DateTime.UtcNow)
            {
                var remainingDays = (student.BannedUntil.Value - DateTime.UtcNow).Days;
                return ErrorResp.Forbidden($"Tài khoản đặt lịch của bạn đang bị khóa tạm thời. Vui lòng thử lại sau {remainingDays} ngày.");
            }

            // Rule 2: Strike 2 quota limit (Max 1 booking per week if strike == 2)
            if (student.StrikesCount == 2)
            {
                var existingBookings = await _bookingRepo.WhereAsync(b => b.StudentId == student.Id && (b.Status == BookingStatusEnum.Pending || b.Status == BookingStatusEnum.Accepted));
                if (existingBookings.Count > 0)
                {
                    return ErrorResp.BadRequest("Tài khoản của bạn đang chịu chế tài Strike 2 (Tối đa 1 cuộc hẹn/tuần). Bạn không thể tạo thêm yêu cầu khi đang có cuộc hẹn hoặc lịch chờ duyệt khác.");
                }
            }

            var mentor = await _mentorRepo.FindByIdAsync(req.MentorId);
            if (mentor == null)
                return ErrorResp.NotFound("Mentor không tồn tại.");

            var slot = await _slotRepo.FindByIdAsync(req.SlotId);
            if (slot == null || !slot.IsAvailable)
                return ErrorResp.BadRequest("Khung giờ hẹn không khả dụng.");

            if (req.IsPriority)
            {
                if (student.PriorityTickets <= 0)
                {
                    return ErrorResp.BadRequest("Bạn không có đủ vé ưu tiên hoạt động.");
                }
                student.PriorityTickets--;
                await _studentRepo.UpdateAsync(student);
            }

            // Rule 3: Pending Lock slot to prevent double-booking
            slot.IsAvailable = false;
            await _slotRepo.UpdateAsync(slot);

            var newBooking = new Bookings
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                MentorId = mentor.Id,
                SlotId = slot.Id,
                BookingTitle = req.BookingTitle,
                SkillGapDescription = req.SkillGapDescription,
                SkillTag = req.SkillTag,
                RequestedTime = DateTime.UtcNow.AddDays(2), // Mock requested time 2 days from now
                IsPriority = req.IsPriority,
                Status = BookingStatusEnum.Pending
            };

            var result = await _bookingRepo.CreateAsync(newBooking);
            
            // Re-fetch to get complete navigations
            var completedBooking = await _bookingRepo.FirstOrDefaultAsync(b => b.Id == result.Id, "Student.User", "Mentor.User", "Slot");
            return SuccessResp.Created(_mapper.Map<BookingDto>(completedBooking));
        }

        public async Task<IActionResult> ListStudentBookings()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId);
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy sinh viên.");

            var bookings = await _bookingRepo.WhereAsync(b => b.StudentId == student.Id, "Student.User", "Mentor.User", "Slot");
            return SuccessResp.Ok(_mapper.Map<List<BookingDto>>(bookings));
        }

        public async Task<IActionResult> ListMentorBookings()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId);
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy mentor.");

            var bookings = await _bookingRepo.WhereAsync(b => b.MentorId == mentor.Id, "Student.User", "Mentor.User", "Slot");
            return SuccessResp.Ok(_mapper.Map<List<BookingDto>>(bookings));
        }

        public async Task<IActionResult> ListAllBookings()
        {
            var bookings = await _bookingRepo.ListAsync("Student.User", "Mentor.User", "Slot");
            return SuccessResp.Ok(_mapper.Map<List<BookingDto>>(bookings));
        }

        public async Task<IActionResult> AcceptBooking(Guid bookingId)
        {
            var booking = await _bookingRepo.FirstOrDefaultAsync(b => b.Id == bookingId, "Student.User", "Mentor.User", "Slot");
            if (booking == null)
                return ErrorResp.NotFound("Không tìm thấy yêu cầu.");

            if (booking.Status != BookingStatusEnum.Pending)
                return ErrorResp.BadRequest("Chỉ có thể chấp nhận các yêu cầu đang chờ duyệt.");

            booking.Status = BookingStatusEnum.Accepted;
            await _bookingRepo.UpdateAsync(booking);

            // Create Google Meet Meeting session
            var newSession = new Sessions
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                StudentId = booking.StudentId,
                MentorId = booking.MentorId,
                MeetingTime = booking.RequestedTime,
                MeetingLink = $"https://meet.google.com/{Guid.NewGuid().ToString().Substring(0, 3)}-{Guid.NewGuid().ToString().Substring(0, 4)}-{Guid.NewGuid().ToString().Substring(0, 3)}",
                IsCompleted = false
            };
            await _sessionRepo.CreateAsync(newSession);

            return SuccessResp.Ok(_mapper.Map<BookingDto>(booking));
        }

        public async Task<IActionResult> DeclineBooking(Guid bookingId, DeclineBookingDto req)
        {
            var booking = await _bookingRepo.FirstOrDefaultAsync(b => b.Id == bookingId, "Student.User", "Mentor.User", "Slot");
            if (booking == null)
                return ErrorResp.NotFound("Không tìm thấy yêu cầu.");

            if (booking.Status != BookingStatusEnum.Pending)
                return ErrorResp.BadRequest("Chỉ có thể từ chối các yêu cầu đang chờ duyệt.");

            booking.Status = BookingStatusEnum.Declined;
            booking.DeclineReason = req.DeclineReason;
            await _bookingRepo.UpdateAsync(booking);

            // Release slot lock
            var slot = await _slotRepo.FindByIdAsync(booking.SlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                await _slotRepo.UpdateAsync(slot);
            }

            // Refund priority ticket if applicable
            if (booking.IsPriority)
            {
                var student = await _studentRepo.FindByIdAsync(booking.StudentId);
                if (student != null)
                {
                    student.PriorityTickets++;
                    await _studentRepo.UpdateAsync(student);
                }
            }

            return SuccessResp.Ok(_mapper.Map<BookingDto>(booking));
        }

        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            var booking = await _bookingRepo.FirstOrDefaultAsync(b => b.Id == bookingId, "Student.User", "Mentor.User", "Slot");
            if (booking == null)
                return ErrorResp.NotFound("Không tìm thấy lịch hẹn.");

            // Rule: Check 12 hours deadline for student cancelation
            var timeDiff = booking.RequestedTime - DateTime.UtcNow;
            if (timeDiff.TotalHours < 12)
            {
                return ErrorResp.BadRequest("Không thể hủy lịch trực tuyến sát giờ (trong vòng 12h trước cuộc gặp). Vui lòng liên hệ trực tiếp Ban Quản Trị Campus.");
            }

            booking.Status = BookingStatusEnum.Canceled;
            await _bookingRepo.UpdateAsync(booking);

            // Release slot lock
            var slot = await _slotRepo.FindByIdAsync(booking.SlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                await _slotRepo.UpdateAsync(slot);
            }

            // Refund ticket if applicable
            if (booking.IsPriority)
            {
                var student = await _studentRepo.FindByIdAsync(booking.StudentId);
                if (student != null)
                {
                    student.PriorityTickets++;
                    await _studentRepo.UpdateAsync(student);
                }
            }

            return SuccessResp.Ok(_mapper.Map<BookingDto>(booking));
        }

        public async Task<IActionResult> EmergencyCancelBooking(Guid bookingId)
        {
            var booking = await _bookingRepo.FirstOrDefaultAsync(b => b.Id == bookingId, "Student.User", "Mentor.User", "Slot");
            if (booking == null)
                return ErrorResp.NotFound("Không tìm thấy lịch hẹn.");

            booking.Status = BookingStatusEnum.Canceled;
            booking.DeclineReason = "Quản trị viên hủy khẩn cấp (Hủy bởi Admin)";
            await _bookingRepo.UpdateAsync(booking);

            // Release slot lock
            var slot = await _slotRepo.FindByIdAsync(booking.SlotId);
            if (slot != null)
            {
                slot.IsAvailable = true;
                await _slotRepo.UpdateAsync(slot);
            }

            // Admin emergency cancelation auto-compensates the student with a priority ticket
            var student = await _studentRepo.FindByIdAsync(booking.StudentId);
            if (student != null)
            {
                student.PriorityTickets++;
                await _studentRepo.UpdateAsync(student);
            }

            return SuccessResp.Ok(_mapper.Map<BookingDto>(booking));
        }
    }
}
