using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfrastructureLayer.Database
{
    public static class DbInitializer
    {
        public static IApplicationBuilder UseInitializeDatabase(this IApplicationBuilder application)
        {
            using var serviceScope = application.ApplicationServices.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetService<MentorBookingDbContext>();

            if (dbContext != null)
            {
                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    Console.WriteLine("Applying Migrations...");
                    dbContext.Database.Migrate();
                }

                try
                {
                    DataSeeder.SeedAdminUser(dbContext).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error seeding data: {ex.Message}");
                }
            }

            return application;
        }
    }
}
