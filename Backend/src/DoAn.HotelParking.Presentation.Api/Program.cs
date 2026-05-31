using DoAn.HotelParking.Core.Application;
using DoAn.HotelParking.Infrastructure;
using DoAn.HotelParking.Infrastructure.Authentication;
using DoAn.HotelParking.Infrastructure.Data.Seeding;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using DoAn.HotelParking.Infrastructure.Notification;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddScoped<LocationSeeder>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "DoAn Hotel Booking API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token in format: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.Configure<FirebaseSettings>(
    builder.Configuration.GetSection("Firebase"));

builder.Services.AddSingleton(sp =>
{
    var firebaseSettings = sp.GetRequiredService<IOptions<FirebaseSettings>>().Value;

    if (string.IsNullOrWhiteSpace(firebaseSettings.CredentialsPath))
        throw new Exception("Firebase CredentialsPath is missing in appsettings.json");

    var fullPath = Path.Combine(AppContext.BaseDirectory, firebaseSettings.CredentialsPath);

    if (!File.Exists(fullPath))
        throw new FileNotFoundException($"Firebase credentials file not found: {fullPath}");

    return FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromFile(fullPath),
        ProjectId = firebaseSettings.ProjectId
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("LocationSeeder");

    try
    {
        var locationSeeder = scope.ServiceProvider.GetRequiredService<LocationSeeder>();
        await locationSeeder.SeedLocationsAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to seed location data.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DoAn Hotel Booking API v1");
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
