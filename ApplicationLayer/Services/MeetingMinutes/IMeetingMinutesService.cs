using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Minutes
{
    public interface IMeetingMinutesService
    {
        Task<IActionResult> SubmitMinutesByStudent(Guid sessionId, SubmitMinutesByStudentDto req);
        Task<IActionResult> SubmitMinutesByMentor(Guid sessionId, SubmitMinutesByMentorDto req);
        Task<IActionResult> GetMinutesBySessionId(Guid sessionId);
        Task<IActionResult> ListAllMinutes();
    }
}
