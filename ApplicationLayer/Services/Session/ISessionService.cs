using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Session
{
    public interface ISessionService
    {
        Task<IActionResult> ListStudentSessions();
        Task<IActionResult> ListMentorSessions();
        Task<IActionResult> ListAllSessions();
        Task<IActionResult> CompleteSession(Guid sessionId);
    }
}
