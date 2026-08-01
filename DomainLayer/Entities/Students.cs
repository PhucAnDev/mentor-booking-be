using System;

namespace DomainLayer.Entities
{
    public class Students : BaseEntity
    {
        public Guid UserId { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public int? SchoolYear { get; set; }
        public string? StartupName { get; set; }
        public string? StartupDescription { get; set; }
        public string? StartupStage { get; set; }
        public string? CvUrl { get; set; }
        public int StrikesCount { get; set; }
        public DateTime? BannedUntil { get; set; }
        public int PriorityTickets { get; set; }

        // Navigation
        public Users User { get; set; } = null!;
    }
}
