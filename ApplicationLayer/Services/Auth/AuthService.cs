using ApplicationLayer.DTOs;
using ApplicationLayer.ResponseCode;
using AutoMapper;
using DomainLayer.Constants;
using DomainLayer.Entities;
using InfrastructureLayer.Core.Crypto;
using InfrastructureLayer.Core.JWT;
using InfrastructureLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static DomainLayer.Enum.GeneralEnum;

namespace ApplicationLayer.Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IGenericRepository<Users> _userRepo;
        private readonly IGenericRepository<Roles> _roleRepo;
        private readonly IGenericRepository<Students> _studentRepo;
        private readonly IGenericRepository<Mentors> _mentorRepo;
        private readonly IJwtService _jwtService;
        private readonly ICryptoService _cryptoService;

        public AuthService(
            IGenericRepository<Users> userRepo,
            IGenericRepository<Roles> roleRepo,
            IGenericRepository<Students> studentRepo,
            IGenericRepository<Mentors> mentorRepo,
            IJwtService jwtService,
            ICryptoService cryptoService,
            IMapper mapper,
            IHttpContextAccessor httpCtx) : base(mapper, httpCtx)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _studentRepo = studentRepo;
            _mentorRepo = mentorRepo;
            _jwtService = jwtService;
            _cryptoService = cryptoService;
        }

        public async Task<IActionResult> Register(RegisterDto req)
        {
            var existUser = await _userRepo.FirstOrDefaultAsync(e => e.Email == req.Email);
            if (existUser != null)
                return ErrorResp.BadRequest("Tài khoản email này đã được đăng ký trước đó.");

            var normalizedRole = req.Role.ToLower().Trim();
            string roleGuidStr = normalizedRole switch
            {
                "admin" => GeneralConst.ADMIN_GUID,
                "mentor" => GeneralConst.ROLE_MENTOR_GUID,
                "enterprise" => GeneralConst.ROLE_ENTERPRISE_GUID,
                _ => GeneralConst.ROLE_STUDENT_GUID // Default to Student
            };

            var roleId = Guid.Parse(roleGuidStr);
            var role = await _roleRepo.FindByIdAsync(roleId);
            if (role == null)
                return ErrorResp.BadRequest("Vai trò đăng ký không tồn tại trên hệ thống.");

            var hashedPassword = _cryptoService.HashPassword(req.Password);

            var newUser = new Users
            {
                Id = Guid.NewGuid(),
                Email = req.Email,
                FullName = req.FullName,
                Password = hashedPassword,
                RoleId = roleId,
                Status = UserStatusEnum.Active
            };

            var createdUser = await _userRepo.CreateAsync(newUser);
            if (createdUser == null)
                return ErrorResp.InternalServerError("Không thể khởi tạo tài khoản.");

            // Create profile associated with Role
            if (roleGuidStr == GeneralConst.ROLE_STUDENT_GUID)
            {
                var newStudent = new Students
                {
                    Id = Guid.NewGuid(),
                    UserId = createdUser.Id,
                    StrikesCount = 0,
                    PriorityTickets = 0
                };
                await _studentRepo.CreateAsync(newStudent);
            }
            else if (roleGuidStr == GeneralConst.ROLE_MENTOR_GUID)
            {
                var newMentor = new Mentors
                {
                    Id = Guid.NewGuid(),
                    UserId = createdUser.Id,
                    Rating = 5.0,
                    IsActive = true
                };
                await _mentorRepo.CreateAsync(newMentor);
            }

            // Map and return
            var userDto = _mapper.Map<UserDto>(createdUser);
            userDto.RoleName = role.Name;
            return SuccessResp.Created(userDto);
        }

        public async Task<IActionResult> Login(LoginDto req)
        {
            var user = await _userRepo.FirstOrDefaultAsync(u => u.Email == req.Email, "Role");
            if (user == null)
                return ErrorResp.Unauthorized("Email hoặc mật khẩu không chính xác.");

            if (user.Status == UserStatusEnum.Suspended || user.Status == UserStatusEnum.Banned)
                return ErrorResp.Forbidden("Tài khoản của bạn đã bị khoá hoặc đình chỉ bởi quản trị viên.");

            var isPasswordValid = _cryptoService.VerifyPassword(req.Password, user.Password);
            if (!isPasswordValid)
                return ErrorResp.Unauthorized("Email hoặc mật khẩu không chính xác.");

            var mockSessionId = Guid.NewGuid();
            var token = _jwtService.GenerateToken(user.Id, user.Role.Name, mockSessionId, user.Email, user.Status, JwtConst.ACCESS_TOKEN_EXP);

            var userDto = _mapper.Map<UserDto>(user);
            var tokenResp = new TokenRespDto
            {
                AccessToken = token,
                User = userDto
            };

            return SuccessResp.Ok(tokenResp);
        }

        public async Task<IActionResult> GetProfile()
        {
            var payload = ExtractPayload();
            if (payload == null)
                return ErrorResp.Unauthorized("Yêu cầu chưa được xác thực.");

            var user = await _userRepo.FirstOrDefaultAsync(u => u.Id == payload.UserId, "Role");
            if (user == null)
                return ErrorResp.NotFound("Không tìm thấy tài khoản.");

            return SuccessResp.Ok(_mapper.Map<UserDto>(user));
        }
    }
}
