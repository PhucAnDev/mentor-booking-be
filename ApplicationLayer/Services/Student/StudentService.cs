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

namespace ApplicationLayer.Services.Student
{
    public class StudentService : BaseService, IStudentService
    {
        private readonly IGenericRepository<Students> _studentRepo;
        private readonly IGenericRepository<Users> _userRepo;
        private readonly IGenericRepository<Bookings> _bookingRepo;

        public StudentService(
            IGenericRepository<Students> studentRepo,
            IGenericRepository<Users> userRepo,
            IGenericRepository<Bookings> bookingRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<IActionResult> GetProfile()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId, "User", "User.Role");
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin sinh viên.");

            return SuccessResp.Ok(_mapper.Map<StudentProfileDto>(student));
        }

        public async Task<IActionResult> UpdateProfile(UpdateStudentProfileDto req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId, "User", "User.Role");
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin sinh viên.");

            // Update user properties
            var user = student.User;
            if (!string.IsNullOrEmpty(req.FullName)) user.FullName = req.FullName;
            if (!string.IsNullOrEmpty(req.Avatar)) user.Avatar = req.Avatar;
            await _userRepo.UpdateAsync(user);

            // Update student properties
            if (req.University != null) student.University = req.University;
            if (req.Major != null) student.Major = req.Major;
            if (req.SchoolYear.HasValue) student.SchoolYear = req.SchoolYear.Value;
            if (req.StartupName != null) student.StartupName = req.StartupName;
            if (req.StartupDescription != null) student.StartupDescription = req.StartupDescription;
            if (req.StartupStage != null) student.StartupStage = req.StartupStage;
            if (req.CvUrl != null) student.CvUrl = req.CvUrl;

            var updatedStudent = await _studentRepo.UpdateAsync(student);
            return SuccessResp.Ok(_mapper.Map<StudentProfileDto>(updatedStudent));
        }

        public async Task<IActionResult> ListStudents()
        {
            var students = await _studentRepo.ListAsync("User", "User.Role");
            return SuccessResp.Ok(_mapper.Map<List<StudentProfileDto>>(students));
        }

        public async Task<IActionResult> UpdateStrikes(Guid studentId, int newStrikesCount)
        {
            var student = await _studentRepo.FirstOrDefaultAsync(s => s.Id == studentId, "User", "User.Role");
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy sinh viên.");

            student.StrikesCount = newStrikesCount;

            if (newStrikesCount >= 3)
            {
                // Ban student for 14 days
                student.BannedUntil = DateTime.UtcNow.AddDays(14);
                
                // Suspend user account
                var user = student.User;
                user.Status = UserStatusEnum.Suspended;
                await _userRepo.UpdateAsync(user);

                // Auto-clear all pending requests
                var pendingRequests = await _bookingRepo.WhereAsync(b => b.StudentId == studentId && b.Status == BookingStatusEnum.Pending);
                if (pendingRequests.Count > 0)
                {
                    await _bookingRepo.DeleteRangeAsync(pendingRequests);
                }
            }
            else
            {
                student.BannedUntil = null;
                var user = student.User;
                user.Status = UserStatusEnum.Active;
                await _userRepo.UpdateAsync(user);
            }

            var updatedStudent = await _studentRepo.UpdateAsync(student);
            return SuccessResp.Ok(_mapper.Map<StudentProfileDto>(updatedStudent));
        }
    }
}
