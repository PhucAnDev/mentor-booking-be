using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Student;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/students")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Protected("Student")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return await _studentService.GetProfile();
        }

        [Protected("Student")]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileDto req)
        {
            return await _studentService.UpdateProfile(req);
        }
    }
}
