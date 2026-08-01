using System;

namespace DomainLayer.Entities
{
    public class Mentors : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public Guid? EnterpriseId { get; set; }
        public double Rating { get; set; }
        public string? Bio { get; set; }
        public string? LinkedinUrl { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public Users User { get; set; } = null!;
        public Enterprises? Enterprise { get; set; }
        public System.Collections.Generic.List<Slots> Slots { get; set; } = new();
    }
}
