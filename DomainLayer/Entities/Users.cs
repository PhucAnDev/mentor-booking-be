using System;
using static DomainLayer.Enum.GeneralEnum;

namespace DomainLayer.Entities
{
    public class Users : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public UserStatusEnum Status { get; set; }
        public string? Avatar { get; set; }

        // Navigation
        public Roles Role { get; set; } = null!;
    }
}
