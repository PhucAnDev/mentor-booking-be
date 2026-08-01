using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using static DomainLayer.Enum.GeneralEnum;

namespace InfrastructureLayer.Core.JWT
{
    public interface IJwtService
    {
        string GenerateToken(Guid userId, string role, Guid sessionId, string email, UserStatusEnum status, int exp);
        Payload? ValidateToken(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly string DEFAULT_SECRET = "f4a8c317e1b0b7f25adf9c04a2b8d8c677c9e3a41fdb8b1e92a05ce6d7415a3e";
        private readonly byte[] _key;
        private readonly JwtSecurityTokenHandler _handler;

        public JwtService()
        {
            var SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ?? DEFAULT_SECRET;
            _key = Encoding.ASCII.GetBytes(SecretKey);
            _handler = new JwtSecurityTokenHandler();
        }

        public string GenerateToken(Guid userId, string role, Guid sessionId, string email, UserStatusEnum status, int exp)
        {
            var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET") ?? DEFAULT_SECRET);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("sessionId", sessionId.ToString()),
                    new Claim("status", status.ToString()),
                    new Claim("email", email),
                    new Claim("role", role)
                }),
                Issuer = userId.ToString(),
                Expires = DateTime.UtcNow.AddSeconds(exp),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = _handler.CreateToken(tokenDescriptor);
            return _handler.WriteToken(token);
        }

        public Payload? ValidateToken(string token)
        {
            _handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var result = (JwtSecurityToken)validatedToken;

            var payload = new Payload
            {
                UserId = Guid.Parse(result.Issuer),
                Email = result.Claims.First(x => x.Type == "email").Value,
                SessionId = Guid.Parse(result.Claims.First(x => x.Type == "sessionId").Value),
                Role = result.Claims.First(x => x.Type == "role").Value,
                Status = Enum.Parse<UserStatusEnum>(result.Claims.First(x => x.Type == "status").Value),
            };

            return payload;
        }
    }
}
