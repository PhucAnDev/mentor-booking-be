using System;

namespace DomainLayer.Entities
{
    public class Sessions : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid StudentId { get; set; }
        public Guid MentorId { get; set; }
        public DateTime MeetingTime { get; set; }
        public string MeetingLink { get; set; } = null!;
        public bool IsCompleted { get; set; }

        // Navigation
        public Bookings Booking { get; set; } = null!;
        public Students Student { get; set; } = null!;
        public Mentors Mentor { get; set; } = null!;
    }
}
