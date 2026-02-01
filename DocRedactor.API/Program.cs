using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DocRedactor.API.Data;
using DocRedactor.API.Interfaces;
using DocRedactor.API.Models;
using DocRedactor.API.Repositories;
using DocRedactor.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services and repositories
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    context.Database.EnsureCreated();
    
    // Seed test users and documents
    await SeedDataAsync(context, userManager);
}

app.Run();

async Task SeedDataAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    // Check if data already exists
    if (context.Users.Any())
    {
        return;
    }

    // Create test users
    var testUsers = new[]
    {
        new { Email = "alice@example.com", UserName = "alice", Password = "Alice123" },
        new { Email = "bob@example.com", UserName = "bob", Password = "Bob123" }
    };

    foreach (var testUser in testUsers)
    {
        var user = new ApplicationUser
        {
            UserName = testUser.UserName,
            Email = testUser.Email
        };

        var result = await userManager.CreateAsync(user, testUser.Password);
        
        if (result.Succeeded)
        {
            // Add test documents for each user
            if (testUser.UserName == "alice")
            {
                var doc1 = new Document
                {
                    Title = "Meeting Notes",
                    Content = "Discussion about Q1 goals and objectives. Key points: increase revenue, improve customer satisfaction.",
                    UserId = user.Id,
                    CurrentVersion = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Documents.Add(doc1);
                await context.SaveChangesAsync();

                var version1 = new DocumentVersion
                {
                    DocumentId = doc1.Id,
                    Content = doc1.Content,
                    VersionNumber = 1,
                    ChangeDescription = "Initial version",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                context.DocumentVersions.Add(version1);

                var doc2 = new Document
                {
                    Title = "Project Proposal",
                    Content = "Proposal for new product development. Timeline: 6 months. Budget: $100,000.",
                    UserId = user.Id,
                    CurrentVersion = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Documents.Add(doc2);
                await context.SaveChangesAsync();

                var version2 = new DocumentVersion
                {
                    DocumentId = doc2.Id,
                    Content = doc2.Content,
                    VersionNumber = 1,
                    ChangeDescription = "Initial version",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                context.DocumentVersions.Add(version2);
            }
            else if (testUser.UserName == "bob")
            {
                var doc1 = new Document
                {
                    Title = "Technical Specification",
                    Content = "System architecture overview. Uses microservices pattern with REST APIs.",
                    UserId = user.Id,
                    CurrentVersion = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                context.Documents.Add(doc1);
                await context.SaveChangesAsync();

                var version1 = new DocumentVersion
                {
                    DocumentId = doc1.Id,
                    Content = doc1.Content,
                    VersionNumber = 1,
                    ChangeDescription = "Initial version",
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow
                };
                context.DocumentVersions.Add(version1);
            }

            await context.SaveChangesAsync();
        }
    }
}
