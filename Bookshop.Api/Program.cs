using Bookshop.Api.Controllers;
using Bookshop.Application.interfaces;
using Bookshop.Application.Services;
using Bookshop.Infrastructure.persistance;
using Bookshop.Infrastructure.Repository;
using Bookshop.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IBookServices, BookServices>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

// ---------- JWT ----------
// "JwtSettings" section একবার পড়ে validate করি; generator আর validation দুটোই একই settings পায়
var jwtSettingsSection = builder.Configuration.GetSection(JwtSettings.SectionName);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? new JwtSettings();
jwtSettings.Validate();
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Claim নামগুলো যেমন আছে তেমন রাখো ("sub", "role" ...), Microsoft-এর লম্বা URI-তে বদলিও না
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.KeyBytes),
            ClockSkew = TimeSpan.Zero,
            NameClaimType = JwtClaimTypes.Name, // User.Identity.Name
            RoleClaimType = JwtClaimTypes.Role  // [Authorize(Roles = "Admin")]
        };

        // Default-এ JwtBearer token খোঁজে "Authorization: Bearer ..." header-এ।
        // Header না থাকলে "accessToken" cookie থেকে পড়ো (browser frontend এটাই ব্যবহার করে)।
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (string.IsNullOrEmpty(context.Request.Headers.Authorization))
                {
                    context.Token = context.Request.Cookies[AuthController.AccessTokenCookieName];
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS ----------
// Frontend (অন্য port) থেকে cookie পাঠাতে হলে AllowCredentials লাগবে, আর origin নির্দিষ্ট করে দিতে হবে
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication(); // আগে "তুমি কে" (cookie-র token চেক)
app.UseAuthorization();  // তারপর "তোমার permission আছে কিনা"

app.MapControllers();

app.Run();
