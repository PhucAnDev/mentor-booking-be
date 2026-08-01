using System;

namespace ApplicationLayer.DTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid MentorId { get; set; }
        public Guid SlotId { get; set; }
        public string BookingTitle { get; set; } = null!;
        public string SkillGapDescription { get; set; } = null!;
        public string SkillTag { get; set; } = null!;
        public DateTime RequestedTime { get; set; }
        public bool IsPriority { get; set; }
        public string Status { get; set; } = null!;
        public string? DeclineReason { get; set; }
        public string StudentName { get; set; } = null!;
        public string MentorName { get; set; } = null!;
        public string TimeSlot { get; set; } = null!;
        public string DayOfWeek { get; set; } = null!;
        public string? StudentEmail { get; set; }
        public int StudentSchoolYear { get; set; }
        public string? StudentUniversity { get; set; }
        public string? StudentCvUrl { get; set; }
    }

    public class CreateBookingDto
    {
        public Guid MentorId { get; set; }
        public Guid SlotId { get; set; }
        public string BookingTitle { get; set; } = null!;
        public string SkillGapDescription { get; set; } = null!;
        public string SkillTag { get; set; } = null!;
        public bool IsPriority { get; set; }
    }

    public class DeclineBookingDto
    {
        public string DeclineReason { get; set; } = null!;
    }
}
