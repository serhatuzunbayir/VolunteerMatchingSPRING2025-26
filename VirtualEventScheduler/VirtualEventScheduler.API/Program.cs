using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VirtualEventScheduler.API.Services;
using VirtualEventScheduler.Data;

var builder = WebApplication.CreateBuilder(args);

// Use a path relative to the executable so the database works on any machine
var dbPath = Path.Combine(AppContext.BaseDirectory, "EventScheduler.db");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// TokenService
builder.Services.AddScoped<TokenService>();

// NotificationService registered as singleton so delegate subscriptions persist across requests
builder.Services.AddSingleton<NotificationService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var admin = db.Users.FirstOrDefault(u => u.Email == "admin@eventapp.com");
    if (admin != null && !BCrypt.Net.BCrypt.Verify("Admin123!", admin.PasswordHash))
    {
        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", 10);
        admin.Role = "Admin";
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
