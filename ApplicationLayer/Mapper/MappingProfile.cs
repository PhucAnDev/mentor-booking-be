using ApplicationLayer.DTOs;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Users mapping
            CreateMap<Users, UserDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Students mapping
            CreateMap<Students, StudentProfileDto>();
            CreateMap<UpdateStudentProfileDto, Students>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Mentors mapping
            CreateMap<Mentors, MentorProfileDto>()
                .ForMember(dest => dest.EnterpriseName, opt => opt.MapFrom(src => src.Enterprise != null ? src.Enterprise.CompanyName : null));
            CreateMap<UpdateMentorProfileDto, Mentors>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Enterprises mapping
            CreateMap<Enterprises, EnterpriseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateEnterpriseDto, Enterprises>();

            // Slots mapping
            CreateMap<Slots, SlotDto>();
            CreateMap<CreateSlotDto, Slots>();

            // Bookings mapping
            CreateMap<Bookings, BookingDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.FullName))
                .ForMember(dest => dest.TimeSlot, opt => opt.MapFrom(src => src.Slot.Time))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.Slot.DayOfWeek));

            // Sessions mapping
            CreateMap<Sessions, SessionDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.FullName))
                .ForMember(dest => dest.BookingTitle, opt => opt.MapFrom(src => src.Booking.BookingTitle));

            // MeetingMinutes mapping
            CreateMap<MeetingMinutes, MeetingMinutesDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.FullName))
                .ForMember(dest => dest.MeetingTime, opt => opt.MapFrom(src => src.Session.MeetingTime.ToString("o")));
        }
    }
}
