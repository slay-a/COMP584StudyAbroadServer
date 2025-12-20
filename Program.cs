using System.Text;
using COMP584StudyAbroadServer.Data;
using COMP584StudyAbroadServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
const string CorsPolicy = "AllowAngularClient";

// ----------------- Services -----------------

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var csFromConfig = builder.Configuration.GetConnectionString("DefaultConnection");

var fallbackConnectionString =
    "Server=studyabroad.ctiqiy8eupkq.us-east-2.rds.amazonaws.com,1433;" +
    "Database=studyabroad-db;" +
    "User Id=admin;" +
    "Password=pkAAlaya1943;" +
    "Encrypt=True;TrustServerCertificate=True;";

// If config is missing/empty, use fallback
var connectionString = string.IsNullOrWhiteSpace(csFromConfig)
    ? fallbackConnectionString
    : csFromConfig;

// DbContext – SQL Server (RDS)
builder.Services.AddDbContext<StudyAbroadContext>(options =>
    options.UseSqlServer(connectionString));

// ASP.NET Core Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<StudyAbroadContext>()
    .AddDefaultTokenProviders();

// JWT auth
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecurityKey"]!))
        };
    });

builder.Services.AddAuthorization();

// CORS – allow browser client (local + deployed)
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// ----------------- Build app -----------------

var app = builder.Build();

// ----- Ensure DB + seed roles/admin + sample data on startup -----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<StudyAbroadContext>();

    // RDS – just ensure created (no delete!)
    await context.Database.EnsureCreatedAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@demo.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    if (!context.Universities.Any())
    {
        var uni1 = new University
        {
            Name = "Global Tech University",
            Country = "USA",
            City = "San Francisco",
            Type = "Public",
            Website = "https://globaltech.edu",
            Description = "Tech-focused university with strong CS and engineering programs."
        };

        var uni2 = new University
        {
            Name = "International Business School",
            Country = "Germany",
            City = "Berlin",
            Type = "Private",
            Website = "https://ibs-berlin.de",
            Description = "Business and management programs with many exchange options."
        };

        context.Universities.AddRange(uni1, uni2);
        await context.SaveChangesAsync();

        context.StudyPrograms.AddRange(
            new StudyProgram
            {
                Name = "BSc Computer Science",
                DegreeLevel = "Bachelor",
                Language = "English",
                DurationMonths = 48,
                TuitionPerYear = 15000,
                IsExchangeFriendly = true,
                UniversityId = uni1.Id
            },
            new StudyProgram
            {
                Name = "MSc Data Science",
                DegreeLevel = "Master",
                Language = "English",
                DurationMonths = 24,
                TuitionPerYear = 18000,
                IsExchangeFriendly = true,
                UniversityId = uni1.Id
            },
            new StudyProgram
            {
                Name = "MBA International Management",
                DegreeLevel = "Master",
                Language = "English",
                DurationMonths = 24,
                TuitionPerYear = 22000,
                IsExchangeFriendly = true,
                UniversityId = uni2.Id
            }
        );

        await context.SaveChangesAsync();
    }
}

// ----------------- Middleware pipeline -----------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();   
app.UseStaticFiles();   

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();
