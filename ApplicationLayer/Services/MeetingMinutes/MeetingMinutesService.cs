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

namespace ApplicationLayer.Services.Minutes
{
    public class MeetingMinutesService : BaseService, IMeetingMinutesService
    {
        private readonly IGenericRepository<MeetingMinutes> _minutesRepo;
        private readonly IGenericRepository<Sessions> _sessionRepo;
        private readonly IGenericRepository<Students> _studentRepo;
        private readonly IGenericRepository<Mentors> _mentorRepo;

        public MeetingMinutesService(
            IGenericRepository<MeetingMinutes> minutesRepo,
            IGenericRepository<Sessions> sessionRepo,
            IGenericRepository<Students> studentRepo,
            IGenericRepository<Mentors> mentorRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _minutesRepo = minutesRepo;
            _sessionRepo = sessionRepo;
            _studentRepo = studentRepo;
            _mentorRepo = mentorRepo;
        }

        public async Task<IActionResult> SubmitMinutesByStudent(Guid sessionId, SubmitMinutesByStudentDto req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var session = await _sessionRepo.FindByIdAsync(sessionId);
            if (session == null)
                return ErrorResp.NotFound("Không tìm thấy cuộc gặp tương ứng.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId);
            if (student == null || session.StudentId != student.Id)
                return ErrorResp.Forbidden("Bạn không sở hữu cuộc gặp này để cập nhật biên bản.");

            var minutes = await _minutesRepo.FirstOrDefaultAsync(m => m.SessionId == sessionId);
            if (minutes == null)
            {
                minutes = new MeetingMinutes
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    StudentId = session.StudentId,
                    MentorId = session.MentorId,
                    RatingByStudent = req.RatingByStudent,
                    ReviewByStudent = req.ReviewByStudent,
                    Summary = req.Summary,
                    ShareWithEnterprise = req.ShareWithEnterprise,
                    SkillVerified = false
                };
                await _minutesRepo.CreateAsync(minutes);
            }
            else
            {
                minutes.RatingByStudent = req.RatingByStudent;
                minutes.ReviewByStudent = req.ReviewByStudent;
                minutes.Summary = req.Summary;
                minutes.ShareWithEnterprise = req.ShareWithEnterprise;
                await _minutesRepo.UpdateAsync(minutes);
            }

            var updatedMinutes = await _minutesRepo.FirstOrDefaultAsync(m => m.Id == minutes.Id, "Student.User", "Mentor.User", "Session");
            return SuccessResp.Ok(_mapper.Map<MeetingMinutesDto>(updatedMinutes));
        }

        public async Task<IActionResult> SubmitMinutesByMentor(Guid sessionId, SubmitMinutesByMentorDto req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var session = await _sessionRepo.FindByIdAsync(sessionId);
            if (session == null)
                return ErrorResp.NotFound("Không tìm thấy cuộc gặp tương ứng.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId);
            if (mentor == null || session.MentorId != mentor.Id)
                return ErrorResp.Forbidden("Bạn không trực tiếp tư vấn cuộc gặp này để cập nhật biên bản.");

            var minutes = await _minutesRepo.FirstOrDefaultAsync(m => m.SessionId == sessionId);
            if (minutes == null)
            {
                minutes = new MeetingMinutes
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    StudentId = session.StudentId,
                    MentorId = session.MentorId,
                    RatingByMentor = req.RatingByMentor,
                    ReviewByMentor = req.ReviewByMentor,
                    Summary = req.Summary,
                    SkillVerified = req.SkillVerified,
                    ShareWithEnterprise = false
                };
                await _minutesRepo.CreateAsync(minutes);
            }
            else
            {
                minutes.RatingByMentor = req.RatingByMentor;
                minutes.ReviewByMentor = req.ReviewByMentor;
                minutes.Summary = req.Summary;
                minutes.SkillVerified = req.SkillVerified;
                await _minutesRepo.UpdateAsync(minutes);
            }

            // Update mentor average rating in DB dynamically!
            var mentorMinutes = await _minutesRepo.WhereAsync(m => m.MentorId == mentor.Id && m.RatingByStudent.HasValue);
            if (mentorMinutes.Count > 0)
            {
                double totalRating = 0;
                foreach (var mm in mentorMinutes)
                {
                    totalRating += mm.RatingByStudent!.Value;
                }
                mentor.Rating = Math.Round(totalRating / mentorMinutes.Count, 1);
                await _mentorRepo.UpdateAsync(mentor);
            }

            var updatedMinutes = await _minutesRepo.FirstOrDefaultAsync(m => m.Id == minutes.Id, "Student.User", "Mentor.User", "Session");
            return SuccessResp.Ok(_mapper.Map<MeetingMinutesDto>(updatedMinutes));
        }

        public async Task<IActionResult> GetMinutesBySessionId(Guid sessionId)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var minutes = await _minutesRepo.FirstOrDefaultAsync(m => m.SessionId == sessionId, "Student.User", "Mentor.User", "Session");
            if (minutes == null)
                return ErrorResp.NotFound("Biên bản cuộc họp chưa được khởi tạo.");

            // Rule 5.4: Access control checks
            if (payload.Role == "Admin")
            {
                // Admin can read all
                return SuccessResp.Ok(_mapper.Map<MeetingMinutesDto>(minutes));
            }

            if (payload.Role == "Student")
            {
                var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId);
                if (student != null && minutes.StudentId == student.Id)
                {
                    return SuccessResp.Ok(_mapper.Map<MeetingMinutesDto>(minutes));
                }
            }

            if (payload.Role == "Mentor")
            {
                var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId);
                if (mentor != null && minutes.MentorId == mentor.Id)
                {
                    return SuccessResp.Ok(_mapper.Map<MeetingMinutesDto>(minutes));
                }
            }

            return ErrorResp.Forbidden("Bạn không có quyền truy cập thông tin biên bản tư vấn này.");
        }

        public async Task<IActionResult> ListAllMinutes()
        {
            var minutes = await _minutesRepo.ListAsync("Student.User", "Mentor.User", "Session");
            return SuccessResp.Ok(_mapper.Map<List<MeetingMinutesDto>>(minutes));
        }
    }
}
