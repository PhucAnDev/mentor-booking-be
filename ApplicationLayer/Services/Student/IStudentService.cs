using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Student
{
    public interface IStudentService
    {
        Task<IActionResult> GetProfile();
        Task<IActionResult> UpdateProfile(UpdateStudentProfileDto req);
        Task<IActionResult> ListStudents();
        Task<IActionResult> UpdateStrikes(Guid studentId, int newStrikesCount);
    }
}
