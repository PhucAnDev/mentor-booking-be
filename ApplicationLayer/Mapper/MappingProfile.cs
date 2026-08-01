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
                .ForMember(dest => dest.EnterpriseName, opt => opt.MapFrom(src => src.Enterprise != null ? src.Enterprise.CompanyName : null))
                .ForMember(dest => dest.Slots, opt => opt.MapFrom(src => src.Slots));
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
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.Slot.DayOfWeek))
                .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => src.Student.User.Email))
                .ForMember(dest => dest.StudentSchoolYear, opt => opt.MapFrom(src => src.Student.SchoolYear))
                .ForMember(dest => dest.StudentUniversity, opt => opt.MapFrom(src => src.Student.University))
                .ForMember(dest => dest.StudentCvUrl, opt => opt.MapFrom(src => src.Student.CvUrl));

            // Sessions mapping
            CreateMap<Sessions, SessionDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.FullName))
                .ForMember(dest => dest.BookingTitle, opt => opt.MapFrom(src => src.Booking.BookingTitle))
                .ForMember(dest => dest.EnterpriseName, opt => opt.MapFrom(src => src.Mentor.Enterprise != null ? src.Mentor.Enterprise.CompanyName : null))
                .ForMember(dest => dest.SkillTag, opt => opt.MapFrom(src => src.Booking.SkillTag))
                .ForMember(dest => dest.IsPriority, opt => opt.MapFrom(src => src.Booking.IsPriority));

            // MeetingMinutes mapping
            CreateMap<MeetingMinutes, MeetingMinutesDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.User.FullName))
                .ForMember(dest => dest.MentorName, opt => opt.MapFrom(src => src.Mentor.User.FullName))
                .ForMember(dest => dest.MeetingTime, opt => opt.MapFrom(src => src.Session.MeetingTime.ToString("o")));
        }
    }
}
