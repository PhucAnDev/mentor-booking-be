using AutoMapper;
using DomainLayer.Constants;
using InfrastructureLayer.Core.JWT;
using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Services
{
    public abstract class BaseService
    {
        protected readonly IMapper _mapper;
        protected readonly IHttpContextAccessor _httpCtx;

        public BaseService(IMapper mapper, IHttpContextAccessor httpCtx)
        {
            _mapper = mapper;
            _httpCtx = httpCtx;
        }

        protected Payload? ExtractPayload()
        {
            var ctx = _httpCtx.HttpContext;
            if (ctx == null) return null;
            var payload = ctx.Items[JwtConst.PAYLOAD_KEY] as Payload;
            return payload;
        }
    }
}
