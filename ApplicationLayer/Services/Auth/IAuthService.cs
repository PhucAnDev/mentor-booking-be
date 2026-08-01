using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Auth
{
    public interface IAuthService
    {
        Task<IActionResult> Register(RegisterDto req);
        Task<IActionResult> Login(LoginDto req);
        Task<IActionResult> GetProfile();
    }
}
