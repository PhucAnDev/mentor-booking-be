using System;
using static DomainLayer.Enum.GeneralEnum;

namespace DomainLayer.Entities
{
    public class Bookings : BaseEntity
    {
        public Guid StudentId { get; set; }
        public Guid MentorId { get; set; }
        public Guid SlotId { get; set; }
        public string BookingTitle { get; set; } = null!;
        public string SkillGapDescription { get; set; } = null!;
        public string SkillTag { get; set; } = null!;
        public DateTime RequestedTime { get; set; }
        public bool IsPriority { get; set; }
        public BookingStatusEnum Status { get; set; }
        public string? DeclineReason { get; set; }

        // Navigation
        public Students Student { get; set; } = null!;
        public Mentors Mentor { get; set; } = null!;
        public Slots Slot { get; set; } = null!;
    }
}
