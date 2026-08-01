using System;

namespace ApplicationLayer.DTOs
{
    public class EnterpriseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Website { get; set; }
        public string? LogoInitials { get; set; }
        public string Status { get; set; } = null!;
    }

    public class CreateEnterpriseDto
    {
        public string CompanyName { get; set; } = null!;
        public string? Website { get; set; }
        public string? LogoInitials { get; set; }
    }
}
