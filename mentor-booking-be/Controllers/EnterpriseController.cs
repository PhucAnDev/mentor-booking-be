using ApplicationLayer.DTOs;
using ApplicationLayer.Middlewares;
using ApplicationLayer.Services.Enterprise;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace mentor_booking_be.Controllers
{
    [ApiController]
    [Route("api/v1/enterprises")]
    public class EnterpriseController : ControllerBase
    {
        private readonly IEnterpriseService _enterpriseService;

        public EnterpriseController(IEnterpriseService enterpriseService)
        {
            _enterpriseService = enterpriseService;
        }

        [HttpGet]
        public async Task<IActionResult> ListEnterprises()
        {
            return await _enterpriseService.ListEnterprises();
        }

        [Protected("Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateEnterprise([FromBody] CreateEnterpriseDto req)
        {
            return await _enterpriseService.CreateEnterprise(req);
        }

        [Protected("Admin")]
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            return await _enterpriseService.ToggleStatus(id);
        }
    }
}
