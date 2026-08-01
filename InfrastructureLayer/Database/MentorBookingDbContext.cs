using DomainLayer.Constants;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using static DomainLayer.Enum.GeneralEnum;

namespace InfrastructureLayer.Database
{
    public class MentorBookingDbContext : DbContext
    {
        public MentorBookingDbContext(DbContextOptions<MentorBookingDbContext> options) : base(options)
        {
        }

        public MentorBookingDbContext()
        {
        }

        public DbSet<Roles> Roles { get; set; } = null!;
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Students> Students { get; set; } = null!;
        public DbSet<Mentors> Mentors { get; set; } = null!;
        public DbSet<Enterprises> Enterprises { get; set; } = null!;
        public DbSet<Slots> Slots { get; set; } = null!;
        public DbSet<Bookings> Bookings { get; set; } = null!;
        public DbSet<Sessions> Sessions { get; set; } = null!;
        public DbSet<MeetingMinutes> MeetingMinutes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<Users>(b =>
            {
                b.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
                b.Property(u => u.Email)
                    .HasMaxLength(100)
                    .IsRequired();
                b.Property(u => u.Password)
                    .HasMaxLength(255)
                    .IsRequired();
                b.Property(u => u.FullName)
                    .HasMaxLength(100)
                    .IsRequired();
                b.Property(u => u.Avatar).IsRequired(false);
                b.Property(e => e.Status).HasDefaultValue(UserStatusEnum.Active);
                b.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                b.Property(x => x.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Roles
            modelBuilder.Entity<Roles>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(20);
                e.Property(x => x.Description).IsRequired(false).HasMaxLength(100);
                e.Property(x => x.Status).HasConversion<string>().HasDefaultValue(RoleStatusEnum.Active);
                e.Property(x => x.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
                e.Property(x => x.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Seed default roles
            modelBuilder.Entity<Roles>().HasData(new Roles
            {
                Id = Guid.Parse(GeneralConst.ADMIN_GUID),
                Name = "Admin",
                Description = "System Administrator",
                Status = RoleStatusEnum.Active,
            }, new Roles
            {
                Id = Guid.Parse(GeneralConst.ROLE_MENTOR_GUID),
                Name = "Mentor",
                Description = "Advisor / Expert",
                Status = RoleStatusEnum.Active,
            }, new Roles
            {
                Id = Guid.Parse(GeneralConst.ROLE_STUDENT_GUID),
                Name = "Student",
                Description = "Student / Startup Founder",
                Status = RoleStatusEnum.Active,
            }, new Roles
            {
                Id = Guid.Parse(GeneralConst.ROLE_ENTERPRISE_GUID),
                Name = "Enterprise",
                Description = "Corporate Partner",
                Status = RoleStatusEnum.Active,
            });
        }
    }
}
