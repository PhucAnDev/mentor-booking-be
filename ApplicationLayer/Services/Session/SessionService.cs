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

namespace ApplicationLayer.Services.Session
{
    public class SessionService : BaseService, ISessionService
    {
        private readonly IGenericRepository<Sessions> _sessionRepo;
        private readonly IGenericRepository<Students> _studentRepo;
        private readonly IGenericRepository<Mentors> _mentorRepo;

        public SessionService(
            IGenericRepository<Sessions> sessionRepo,
            IGenericRepository<Students> studentRepo,
            IGenericRepository<Mentors> mentorRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _sessionRepo = sessionRepo;
            _studentRepo = studentRepo;
            _mentorRepo = mentorRepo;
        }

        public async Task<IActionResult> ListStudentSessions()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var student = await _studentRepo.FirstOrDefaultAsync(s => s.UserId == payload.UserId);
            if (student == null)
                return ErrorResp.NotFound("Không tìm thấy sinh viên.");

            var sessions = await _sessionRepo.WhereAsync(s => s.StudentId == student.Id, "Student.User", "Mentor.User", "Booking");
            return SuccessResp.Ok(_mapper.Map<List<SessionDto>>(sessions));
        }

        public async Task<IActionResult> ListMentorSessions()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId);
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy mentor.");

            var sessions = await _sessionRepo.WhereAsync(s => s.MentorId == mentor.Id, "Student.User", "Mentor.User", "Booking");
            return SuccessResp.Ok(_mapper.Map<List<SessionDto>>(sessions));
        }

        public async Task<IActionResult> ListAllSessions()
        {
            var sessions = await _sessionRepo.ListAsync("Student.User", "Mentor.User", "Booking");
            return SuccessResp.Ok(_mapper.Map<List<SessionDto>>(sessions));
        }

        public async Task<IActionResult> CompleteSession(Guid sessionId)
        {
            var session = await _sessionRepo.FirstOrDefaultAsync(s => s.Id == sessionId, "Student.User", "Mentor.User", "Booking");
            if (session == null)
                return ErrorResp.NotFound("Không tìm thấy cuộc gặp.");

            session.IsCompleted = true;
            var result = await _sessionRepo.UpdateAsync(session);
            return SuccessResp.Ok(_mapper.Map<SessionDto>(result));
        }
    }
}
