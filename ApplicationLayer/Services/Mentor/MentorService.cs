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

namespace ApplicationLayer.Services.Mentor
{
    public class MentorService : BaseService, IMentorService
    {
        private readonly IGenericRepository<Mentors> _mentorRepo;
        private readonly IGenericRepository<Users> _userRepo;
        private readonly IGenericRepository<Slots> _slotRepo;

        public MentorService(
            IGenericRepository<Mentors> mentorRepo,
            IGenericRepository<Users> userRepo,
            IGenericRepository<Slots> slotRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _mentorRepo = mentorRepo;
            _userRepo = userRepo;
            _slotRepo = slotRepo;
        }

        public async Task<IActionResult> GetProfile()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId, "User", "User.Role", "Enterprise", "Slots");
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin mentor.");

            return SuccessResp.Ok(_mapper.Map<MentorProfileDto>(mentor));
        }

        public async Task<IActionResult> UpdateProfile(UpdateMentorProfileDto req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId, "User", "User.Role", "Enterprise");
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin mentor.");

            // Update user properties
            var user = mentor.User;
            if (!string.IsNullOrEmpty(req.FullName)) user.FullName = req.FullName;
            if (!string.IsNullOrEmpty(req.Avatar)) user.Avatar = req.Avatar;
            await _userRepo.UpdateAsync(user);

            // Update mentor properties
            if (req.Title != null) mentor.Title = req.Title;
            if (req.Bio != null) mentor.Bio = req.Bio;
            if (req.LinkedinUrl != null) mentor.LinkedinUrl = req.LinkedinUrl;
            if (req.EnterpriseId.HasValue) mentor.EnterpriseId = req.EnterpriseId.Value;

            var updatedMentor = await _mentorRepo.UpdateAsync(mentor);
            var resultDto = _mapper.Map<MentorProfileDto>(updatedMentor);
            return SuccessResp.Ok(resultDto);
        }

        public async Task<IActionResult> ListActiveMentors()
        {
            var mentors = await _mentorRepo.WhereAsync(
                m => m.IsActive && m.User.Status == UserStatusEnum.Active,
                "User", "User.Role", "Enterprise", "Slots"
            );
            return SuccessResp.Ok(_mapper.Map<List<MentorProfileDto>>(mentors));
        }

        public async Task<IActionResult> ListAllMentors()
        {
            var mentors = await _mentorRepo.ListAsync("User", "User.Role", "Enterprise", "Slots");
            return SuccessResp.Ok(_mapper.Map<List<MentorProfileDto>>(mentors));
        }

        public async Task<IActionResult> ToggleActivationStatus(Guid mentorId)
        {
            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.Id == mentorId, "User", "User.Role", "Enterprise", "Slots");
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin mentor.");

            mentor.IsActive = !mentor.IsActive;
            var updatedMentor = await _mentorRepo.UpdateAsync(mentor);
            return SuccessResp.Ok(_mapper.Map<MentorProfileDto>(updatedMentor));
        }

        public async Task<IActionResult> GetSlots(Guid mentorId)
        {
            var slots = await _slotRepo.WhereAsync(s => s.MentorId == mentorId);
            return SuccessResp.Ok(_mapper.Map<List<SlotDto>>(slots));
        }

        public async Task<IActionResult> UpdateSlots(List<CreateSlotDto> req)
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var mentor = await _mentorRepo.FirstOrDefaultAsync(m => m.UserId == payload.UserId);
            if (mentor == null)
                return ErrorResp.NotFound("Không tìm thấy thông tin mentor.");

            // Clear old slots
            var oldSlots = await _slotRepo.WhereAsync(s => s.MentorId == mentor.Id);
            if (oldSlots.Count > 0)
            {
                await _slotRepo.DeleteRangeAsync(oldSlots);
            }

            // Add new slots
            var newSlots = new List<Slots>();
            foreach (var slotDto in req)
            {
                newSlots.Add(new Slots
                {
                    Id = Guid.NewGuid(),
                    MentorId = mentor.Id,
                    DayOfWeek = slotDto.DayOfWeek,
                    Time = slotDto.Time,
                    IsAvailable = true
                });
            }

            if (newSlots.Count > 0)
            {
                await _slotRepo.CreateRangeAsync(newSlots);
            }

            return SuccessResp.Ok(_mapper.Map<List<SlotDto>>(newSlots));
        }
    }
}
