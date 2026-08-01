using System;
using static DomainLayer.Enum.GeneralEnum;

namespace DomainLayer.Entities
{
    public class Enterprises : BaseEntity
    {
        public string CompanyName { get; set; } = null!;
        public string? Website { get; set; }
        public string? LogoInitials { get; set; }
        public CompanyStatusEnum Status { get; set; }
    }
}
