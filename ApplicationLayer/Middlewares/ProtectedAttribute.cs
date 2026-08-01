using System;
using System.Linq;
using ApplicationLayer.ResponseCode;
using DomainLayer.Constants;
using InfrastructureLayer.Core.JWT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ApplicationLayer.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ProtectedAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IJwtService _jwtService;
        private readonly string[] _allowedRoles;

        public ProtectedAttribute(params string[] allowedRoles)
        {
            _jwtService = new JwtService();
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = ErrorResp.Unauthorized("Vui lòng cung cấp mã xác thực JWT Bearer Token hợp lệ.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length);

            try
            {
                var payload = _jwtService.ValidateToken(token);
                if (payload == null)
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = ErrorResp.Unauthorized("Mã xác thực không hợp lệ hoặc đã hết hạn.");
                    return;
                }

                // Role check
                if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(payload.Role))
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Result = ErrorResp.Forbidden("Tài khoản của bạn không được phân quyền truy cập chức năng này.");
                    return;
                }

                // Add payload to HttpContext
                context.HttpContext.Items[JwtConst.PAYLOAD_KEY] = payload;
            }
            catch (Exception e)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = ErrorResp.Unauthorized($"Mã xác thực không hợp lệ: {e.Message}");
                return;
            }
        }
    }
}
