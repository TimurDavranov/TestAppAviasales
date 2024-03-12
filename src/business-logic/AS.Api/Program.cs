using AS.Core.Configurations;
using AS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IIdentityService, IdentityService>();
var settings = new ApplicationSettings();
builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(settings);
builder.Services.AddSingleton(settings);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
