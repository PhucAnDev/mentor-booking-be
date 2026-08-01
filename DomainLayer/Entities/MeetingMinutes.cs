using System;

namespace DomainLayer.Entities
{
    public class MeetingMinutes : BaseEntity
    {
        public Guid SessionId { get; set; }
        public Guid StudentId { get; set; }
        public Guid MentorId { get; set; }
        public int? RatingByStudent { get; set; }
        public string? ReviewByStudent { get; set; }
        public int? RatingByMentor { get; set; }
        public string? ReviewByMentor { get; set; }
        public string? Summary { get; set; }
        public bool SkillVerified { get; set; }
        public bool ShareWithEnterprise { get; set; }

        // Navigation
        public Sessions Session { get; set; } = null!;
        public Students Student { get; set; } = null!;
        public Mentors Mentor { get; set; } = null!;
    }
}
