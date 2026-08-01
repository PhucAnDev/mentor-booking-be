using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto req)
        {
            return await _authService.Register(req);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req)
        {
            return await _authService.Login(req);
        }

        [Protected]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return await _authService.GetProfile();
        }
    }
}
