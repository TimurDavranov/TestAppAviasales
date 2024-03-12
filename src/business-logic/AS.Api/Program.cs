using AS.Api.Services;
using AS.Core.Configurations;
using AS.Core.Factories;
using AS.Core.Helpers;
using AS.Core.Services;
using AS.Identity.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<IDDbContext>();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.SetIsOriginAllowed(a => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

var settings = new ApplicationSettings();
builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(settings);
builder.Services.AddSingleton(settings);

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddSingleton<IRabbitConnection, RabbitConnection>();
builder.Services.AddSingleton<IRabbitMessageProducer, RabbitMessageProducer>();
builder.Services.AddSingleton<RedisService>();

Action<DbContextOptionsBuilder> dbOptions =
    opt => opt
        .UseSqlServer(settings.DBConnectionString,
            o => o.EnableRetryOnFailure())
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors();
builder.Services.AddDbContext<IDDbContext>(dbOptions, ServiceLifetime.Scoped);
builder.Services.AddSingleton(new DatabaseContextFactory<IdentityDbContext>(dbOptions));

var app = builder.Build();

app.UseSwagger()
    .UseAuthentication()
    .UseAuthorization();
app.UseSwaggerUI()
    .UseAuthentication()
    .UseAuthorization();

app.UseCors("AllowAll");
app.MapIdentityApi<IdentityUser>();
app.UseAuthentication();
app.UseAuthorization();
app.MapSwagger()
    .RequireAuthorization();
app.MapControllers();

app.Run();