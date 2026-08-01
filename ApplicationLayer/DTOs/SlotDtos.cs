using System;

namespace ApplicationLayer.DTOs
{
    public class SlotDto
    {
        public Guid Id { get; set; }
        public Guid MentorId { get; set; }
        public string DayOfWeek { get; set; } = null!;
        public string Time { get; set; } = null!;
        public bool IsAvailable { get; set; }
    }

    public class CreateSlotDto
    {
        public string DayOfWeek { get; set; } = null!;
        public string Time { get; set; } = null!;
    }
}
