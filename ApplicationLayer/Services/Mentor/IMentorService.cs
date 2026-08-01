using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Mentor
{
    public interface IMentorService
    {
        Task<IActionResult> GetProfile();
        Task<IActionResult> UpdateProfile(UpdateMentorProfileDto req);
        Task<IActionResult> ListActiveMentors();
        Task<IActionResult> ListAllMentors();
        Task<IActionResult> ToggleActivationStatus(Guid mentorId);
        Task<IActionResult> GetSlots(Guid mentorId);
        Task<IActionResult> UpdateSlots(List<CreateSlotDto> req);
    }
}
