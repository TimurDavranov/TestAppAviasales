using AS.Core.Configurations;
using AS.Identity.Api;
using AS.Identity.Api.Domain;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var settings = new ApplicationSettings();
builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(settings);
builder.Services.AddSingleton(settings);

builder.Services.ConfigureDI(settings);

builder.Services.AddAuthorization();
builder.Services
    .AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapIdentityApi<IdentityUser>();
app.UseAuthorization();

app
    .MapSwagger()
    .RequireAuthorization();

app.MapControllers();

app.Run();
