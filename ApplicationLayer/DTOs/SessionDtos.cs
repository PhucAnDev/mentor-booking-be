using System;

namespace ApplicationLayer.DTOs
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid StudentId { get; set; }
        public Guid MentorId { get; set; }
        public DateTime MeetingTime { get; set; }
        public string MeetingLink { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public string StudentName { get; set; } = null!;
        public string MentorName { get; set; } = null!;
        public string BookingTitle { get; set; } = null!;
        public string? EnterpriseName { get; set; }
        public string? SkillTag { get; set; }
        public bool IsPriority { get; set; }
    }
}
