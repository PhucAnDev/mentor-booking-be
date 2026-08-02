using ApplicationLayer.Mapper;
using ApplicationLayer.Services.Auth;
using ApplicationLayer.Services.Student;
using ApplicationLayer.Services.Mentor;
using ApplicationLayer.Services.Enterprise;
using ApplicationLayer.Services.Booking;
using ApplicationLayer.Services.Session;
using ApplicationLayer.Services.Minutes;
using InfrastructureLayer.Core.Cache;
using InfrastructureLayer.Core.Crypto;
using InfrastructureLayer.Core.JWT;
using InfrastructureLayer.Core.Mail;
using InfrastructureLayer.Database;
using InfrastructureLayer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000",
                "https://ca-mentor-booking-frontend.delightfulcoast-cbd81cee.japaneast.azurecontainerapps.io"
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// JWT Authentication Setup
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? "ea8cf10696dc45a8b7b5f15758ae3ef238b440cfa1f84b449af315d515de6f95";
var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret));

builder.Services.AddAuthentication("Bearer")
    .AddBearerToken()
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// DbContext configuration (MS SQL Server)
builder.Services.AddDbContext<MentorBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Redis multiplexer (optional — app continues without cache if Redis is unavailable)
var redisConn = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
try
{
    var redisOptions = ConfigurationOptions.Parse(redisConn);
    redisOptions.ConnectTimeout = 3000;
    redisOptions.AbortOnConnectFail = false;
    builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisOptions));
    builder.Services.AddScoped<ICacheService, CacheService>();
    Console.WriteLine("Redis connected successfully.");
}
catch (Exception redisEx)
{
    Console.WriteLine($"[WARN] Redis unavailable ({redisConn}): {redisEx.Message}. Running without cache.");
    // Register a no-op / null cache service so DI still resolves ICacheService
    builder.Services.AddScoped<ICacheService, NullCacheService>();
}

// CORE Services
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddSingleton<ICryptoService, CryptoService>();

var smtpUsername = builder.Configuration.GetValue<string>("SMTPEmail") ?? "smtp_email";
var smtpPassword = builder.Configuration.GetValue<string>("SMTPPassword") ?? "smtp_password";
builder.Services.AddSingleton<IMailService>(new MailService("smtp.gmail.com", 587, smtpUsername, smtpPassword));

// Application Services DI registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<IEnterpriseService, EnterpriseService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IMeetingMinutesService, MeetingMinutesService>();

// Swagger configuration with JWT Bearer support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mentor Booking API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Automatically apply migration and seed default admin user
using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Checking database...");
    var context = scope.ServiceProvider.GetRequiredService<MentorBookingDbContext>();
    try
    {
        Console.WriteLine("Applying pending migrations...");
        context.Database.Migrate();
        Console.WriteLine("Database is up-to-date!");
        await DataSeeder.SeedAdminUser(context);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database migration or seeding: {ex.Message}");
    }
}

// Swagger enabled in all environments for API access
app.UseSwagger();
app.UseSwaggerUI();


app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
