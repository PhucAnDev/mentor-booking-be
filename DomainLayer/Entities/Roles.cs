using System;
using static DomainLayer.Enum.GeneralEnum;

namespace DomainLayer.Entities
{
    public class Roles : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public RoleStatusEnum? Status { get; set; }
    }
}
