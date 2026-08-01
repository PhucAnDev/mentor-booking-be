using System;
using static DomainLayer.Enum.GeneralEnum;

namespace InfrastructureLayer.Core.JWT
{
    public class Payload
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public Guid SessionId { get; set; }
        public string Role { get; set; } = null!;
        public UserStatusEnum Status { get; set; }
    }
}
