using System;

namespace DomainLayer.Entities
{
    public class Slots : BaseEntity
    {
        public Guid MentorId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public string Time { get; set; } = null!;
        public bool IsAvailable { get; set; }

        // Navigation
        public Mentors Mentor { get; set; } = null!;
    }
}
