using System;

namespace ApplicationLayer.DTOs
{
    public class MentorProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public Guid? EnterpriseId { get; set; }
        public double Rating { get; set; }
        public string? Bio { get; set; }
        public string? LinkedinUrl { get; set; }
        public bool IsActive { get; set; }
        public UserDto User { get; set; } = null!;
        public string? EnterpriseName { get; set; }
    }

    public class UpdateMentorProfileDto
    {
        public string? FullName { get; set; }
        public string? Avatar { get; set; }
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? LinkedinUrl { get; set; }
        public Guid? EnterpriseId { get; set; }
    }
}
