using System;

namespace ApplicationLayer.DTOs
{
    public class MeetingMinutesDto
    {
        public Guid Id { get; set; }
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
        public string StudentName { get; set; } = null!;
        public string MentorName { get; set; } = null!;
        public string MeetingTime { get; set; } = null!;
    }

    public class SubmitMinutesByStudentDto
    {
        public int RatingByStudent { get; set; }
        public string? ReviewByStudent { get; set; }
        public string? Summary { get; set; }
        public bool ShareWithEnterprise { get; set; }
    }

    public class SubmitMinutesByMentorDto
    {
        public int RatingByMentor { get; set; }
        public string? ReviewByMentor { get; set; }
        public string? Summary { get; set; }
        public bool SkillVerified { get; set; }
    }
}
