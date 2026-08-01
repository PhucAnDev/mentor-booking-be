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
    }
}
