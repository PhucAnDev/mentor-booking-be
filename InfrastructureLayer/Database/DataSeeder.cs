using DomainLayer.Constants;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using static DomainLayer.Enum.GeneralEnum;

namespace InfrastructureLayer.Database
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUser(MentorBookingDbContext context)
        {
            try
            {
                Console.WriteLine(" Đang kiểm tra tài khoản admin...");
                var existingAdmin = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == "admin@campus.edu.vn");

                if (existingAdmin == null)
                {
                    Console.WriteLine("Đang tạo tài khoản admin mặc định...");

                    var adminUser = new Users
                    {
                        Id = Guid.Parse(GeneralConst.ADMIN_GUID),
                        RoleId = Guid.Parse(GeneralConst.ADMIN_GUID),
                        Email = "admin@campus.edu.vn",
                        Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        FullName = "Administrator",
                        Status = UserStatusEnum.Active,
                        Avatar = "https://static.vecteezy.com/system/resources/previews/029/156/453/non_2x/admin-business-icon-businessman-business-people-male-avatar-profile-pictures-man-in-suit-for-your-web-site-design-logo-app-ui-solid-style-illustration-design-on-white-background-eps-10-vector.jpg"
                    };

                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();

                    Console.WriteLine("   Tài khoản admin đã được tạo thành công!");
                    Console.WriteLine("   Email: admin@campus.edu.vn");
                    Console.WriteLine("   Password: admin123");
                }
                else
                {
                    Console.WriteLine("  Tài khoản admin đã tồn tại, đang cập nhật password...");
                    existingAdmin.Password = BCrypt.Net.BCrypt.HashPassword("admin123");
                    existingAdmin.UpdatedAt = DateTime.UtcNow;

                    context.Users.Update(existingAdmin);
                    await context.SaveChangesAsync();

                    Console.WriteLine(" Password admin đã được cập nhật!");
                    Console.WriteLine("   Email: admin@campus.edu.vn");
                    Console.WriteLine("   Password: admin123");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo tài khoản admin: {ex.Message}");
            }
        }

        public static async Task SeedPastCompletedSession(MentorBookingDbContext context)
        {
            try
            {
                // 1. Ensure default Student Users exist (student@gmail.com, student@edu.vn, an.nv2100@student.edu.vn)
                var studentEmails = new[] { "student@gmail.com", "an.nv2100@student.edu.vn", "student@edu.vn" };
                Students? defaultStudent = null;

                foreach (var email in studentEmails)
                {
                    var pass = email.Contains("gmail.com") ? "123456789" : "123456";
                    var sUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (sUser == null)
                    {
                        sUser = new Users
                        {
                            Id = Guid.NewGuid(),
                            RoleId = Guid.Parse(GeneralConst.ROLE_STUDENT_GUID),
                            Email = email,
                            Password = BCrypt.Net.BCrypt.HashPassword(pass),
                            FullName = "Nguyễn Văn An",
                            Status = UserStatusEnum.Active,
                            Avatar = "https://images.unsplash.com/photo-1539571696357-5a69c17a67c6?auto=format&fit=crop&q=80&w=120"
                        };
                        await context.Users.AddAsync(sUser);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        sUser.Password = BCrypt.Net.BCrypt.HashPassword(pass);
                        context.Users.Update(sUser);
                        await context.SaveChangesAsync();
                    }

                    var sEntity = await context.Students.FirstOrDefaultAsync(s => s.UserId == sUser.Id);
                    if (sEntity == null)
                    {
                        sEntity = new Students
                        {
                            Id = Guid.NewGuid(),
                            UserId = sUser.Id,
                            University = "Đại học Bách Khoa",
                            Major = "Kỹ thuật Phần mềm",
                            SchoolYear = 3,
                            StartupName = "CampusConnect AI",
                            StartupDescription = "Nền tảng kết nối cố vấn hướng nghiệp cho sinh viên.",
                            StartupStage = "Ý tưởng (Idea Stage)",
                            CvUrl = "CV_NguyenVanAn_SoftwareEngineer.pdf",
                            StrikesCount = 0,
                            PriorityTickets = 1
                        };
                        await context.Students.AddAsync(sEntity);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        sEntity.University = "Đại học Bách Khoa";
                        sEntity.Major = "Kỹ thuật Phần mềm";
                        sEntity.SchoolYear = 3;
                        sEntity.StartupName = "CampusConnect AI";
                        sEntity.StartupDescription = "Nền tảng kết nối cố vấn hướng nghiệp cho sinh viên.";
                        sEntity.StartupStage = "Ý tưởng (Idea Stage)";
                        context.Students.Update(sEntity);
                        await context.SaveChangesAsync();
                    }
                    if (defaultStudent == null) defaultStudent = sEntity;
                }

                // 2. Ensure default Mentor Users exist (mentor@gmail.com, mentor@fpt.edu.vn, mentor@campus.edu.vn)
                var mentorEmails = new[] { "mentor@gmail.com", "mentor@fpt.edu.vn", "mentor@campus.edu.vn" };
                foreach (var email in mentorEmails)
                {
                    var pass = email.Contains("gmail.com") ? "123456789" : "123456";
                    var mUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                    if (mUser == null)
                    {
                        mUser = new Users
                        {
                            Id = Guid.NewGuid(),
                            RoleId = Guid.Parse(GeneralConst.ROLE_MENTOR_GUID),
                            Email = email,
                            Password = BCrypt.Net.BCrypt.HashPassword(pass),
                            FullName = "Trần Đức Bằng",
                            Status = UserStatusEnum.Active,
                            Avatar = "https://images.unsplash.com/photo-1560250097-0b93528c311a?auto=format&fit=crop&q=80&w=120"
                        };
                        await context.Users.AddAsync(mUser);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        mUser.Password = BCrypt.Net.BCrypt.HashPassword(pass);
                        context.Users.Update(mUser);
                        await context.SaveChangesAsync();
                    }

                    var mEntity = await context.Mentors.FirstOrDefaultAsync(m => m.UserId == mUser.Id);
                    if (mEntity == null)
                    {
                        mEntity = new Mentors
                        {
                            Id = Guid.NewGuid(),
                            UserId = mUser.Id,
                            Title = "Senior Fullstack Consultant",
                            Rating = 5.0,
                            Bio = "Hơn 8 năm kinh nghiệm phát triển phần mềm và tư vấn định hướng sự nghiệp IT.",
                            LinkedinUrl = "https://linkedin.com/in/mentor-demo",
                            IsActive = true
                        };
                        await context.Mentors.AddAsync(mEntity);
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        mEntity.Title = "Senior Fullstack Consultant";
                        mEntity.Rating = 5.0;
                        mEntity.Bio = "Hơn 8 năm kinh nghiệm phát triển phần mềm và tư vấn định hướng sự nghiệp IT.";
                        mEntity.LinkedinUrl = "https://linkedin.com/in/mentor-demo";
                        mEntity.IsActive = true;
                        context.Mentors.Update(mEntity);
                        await context.SaveChangesAsync();
                    }
                }

                // 3. For ALL mentors in DB, ensure each has a completed Session & MeetingMinutes
                var allMentors = await context.Mentors.Include(m => m.User).ToListAsync();
                foreach (var mentor in allMentors)
                {
                    var hasMinutes = await context.MeetingMinutes.AnyAsync(m => m.MentorId == mentor.Id);
                    if (!hasMinutes && defaultStudent != null)
                    {
                        var slot = new Slots
                        {
                            Id = Guid.NewGuid(),
                            MentorId = mentor.Id,
                            DayOfWeek = "Thứ 2",
                            Time = "09:00 - 10:00",
                            IsAvailable = false
                        };
                        await context.Slots.AddAsync(slot);
                        await context.SaveChangesAsync();

                        var pastTime = DateTime.UtcNow.AddDays(-3);
                        var booking = new Bookings
                        {
                            Id = Guid.NewGuid(),
                            StudentId = defaultStudent.Id,
                            MentorId = mentor.Id,
                            SlotId = slot.Id,
                            BookingTitle = "Tư vấn lộ trình React & TypeScript nâng cao",
                            SkillGapDescription = "Cần tham vấn lộ trình học tập để ứng tuyển vị trí Frontend Developer.",
                            SkillTag = "React Components & State",
                            RequestedTime = pastTime,
                            IsPriority = false,
                            Status = BookingStatusEnum.Accepted
                        };
                        await context.Bookings.AddAsync(booking);
                        await context.SaveChangesAsync();

                        var session = new Sessions
                        {
                            Id = Guid.NewGuid(),
                            BookingId = booking.Id,
                            StudentId = defaultStudent.Id,
                            MentorId = mentor.Id,
                            MeetingTime = pastTime,
                            MeetingLink = "https://meet.google.com/abc-defg-hij",
                            IsCompleted = true
                        };
                        await context.Sessions.AddAsync(session);
                        await context.SaveChangesAsync();

                        var minutes = new MeetingMinutes
                        {
                            Id = Guid.NewGuid(),
                            SessionId = session.Id,
                            StudentId = defaultStudent.Id,
                            MentorId = mentor.Id,
                            RatingByStudent = 5,
                            ReviewByStudent = "Buổi tư vấn cực kỳ chất lượng! Anh Mentor giải thích mạch lạc về React State & Custom Hooks, chỉ ra đúng các lỗ hổng kiến thức và hướng dẫn cách hoàn thiện CV.",
                            RatingByMentor = 5,
                            ReviewByMentor = "Sinh viên Nguyễn Văn An có nền tảng tư duy lập trình tốt, thái độ cầu thị và chuẩn bị sẵn các câu hỏi cụ thể trước khi tham gia buổi hẹn. Đã hoàn thành đánh giá kỹ năng.",
                            Summary = "Đã thống nhất lộ trình học tập React & TypeScript trong 4 tuần. Sinh viên ghi nhận hành động: Thực hành refactor code component sang TypeScript, Xây dựng dự án portfolio cá nhân.",
                            SkillVerified = true,
                            ShareWithEnterprise = true
                        };
                        await context.MeetingMinutes.AddAsync(minutes);
                        await context.SaveChangesAsync();
                    }
                }

                Console.WriteLine(" Đã đảm bảo tất cả tài khoản Mentor (bao gồm mentor@fpt.edu.vn) đều có dữ liệu Nhật ký tư vấn mẫu!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi seed dữ liệu cuộc hẹn đã kết thúc: {ex.Message}");
            }
        }
    }
}
