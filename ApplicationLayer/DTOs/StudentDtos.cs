using System;

namespace ApplicationLayer.DTOs
{
    public class StudentProfileDto
    {
        public Guid Id { get; set; }
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
        public UserDto User { get; set; } = null!;
    }

    public class UpdateStudentProfileDto
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? University { get; set; }
        public string? Major { get; set; }
        public int? SchoolYear { get; set; }
        public string? StartupName { get; set; }
        public string? StartupDescription { get; set; }
        public string? StartupStage { get; set; }
        public string? CvUrl { get; set; }
    }
}
