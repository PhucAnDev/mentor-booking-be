using ApplicationLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApplicationLayer.Services.Enterprise
{
    public interface IEnterpriseService
    {
        Task<IActionResult> ListEnterprises();
        Task<IActionResult> CreateEnterprise(CreateEnterpriseDto req);
        Task<IActionResult> ToggleStatus(Guid id);
    }
}
