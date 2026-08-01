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

namespace ApplicationLayer.Services.Enterprise
{
    public class EnterpriseService : BaseService, IEnterpriseService
    {
        private readonly IGenericRepository<Enterprises> _enterpriseRepo;

        public EnterpriseService(
            IGenericRepository<Enterprises> enterpriseRepo,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _enterpriseRepo = enterpriseRepo;
        }

        public async Task<IActionResult> ListEnterprises()
        {
            var enterprises = await _enterpriseRepo.ListAsync();
            return SuccessResp.Ok(_mapper.Map<List<EnterpriseDto>>(enterprises));
        }

        public async Task<IActionResult> CreateEnterprise(CreateEnterpriseDto req)
        {
            var newEnterprise = _mapper.Map<Enterprises>(req);
            newEnterprise.Id = Guid.NewGuid();
            newEnterprise.Status = CompanyStatusEnum.Active;

            var result = await _enterpriseRepo.CreateAsync(newEnterprise);
            return SuccessResp.Created(_mapper.Map<EnterpriseDto>(result));
        }

        public async Task<IActionResult> ToggleStatus(Guid id)
        {
            var enterprise = await _enterpriseRepo.FindByIdAsync(id);
            if (enterprise == null)
                return ErrorResp.NotFound("Không tìm thấy doanh nghiệp.");

            enterprise.Status = enterprise.Status == CompanyStatusEnum.Active ? CompanyStatusEnum.Inactive : CompanyStatusEnum.Active;
            var result = await _enterpriseRepo.UpdateAsync(enterprise);

            return SuccessResp.Ok(_mapper.Map<EnterpriseDto>(result));
        }
    }
}
