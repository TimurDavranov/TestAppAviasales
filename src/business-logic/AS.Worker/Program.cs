using AS.Application.Handlers;
using AS.Application.Repositories;
using AS.Core.Configurations;
using AS.Core.Events;
using AS.Core.Factories;
using AS.Core.Helpers;
using AS.Domain;
using AS.Worker.Services.BackgroudServices;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<MockDataBackgroundService>();

var settings = new ApplicationSettings();
builder.Configuration.GetSection(nameof(ApplicationSettings)).Bind(settings);
builder.Services.AddSingleton(settings);

Action<DbContextOptionsBuilder> dbOptions =
                opt => opt
                        .UseSqlServer(settings.DBConnectionString,
                            o => o.EnableRetryOnFailure())
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();

builder.Services
            .AddDbContext<IApplicationDbContext, ApplicationDbContext>(dbOptions, ServiceLifetime.Scoped);
builder.Services.AddSingleton(new DatabaseContextFactory<ApplicationDbContext>(dbOptions));

builder.Services.AddScoped<IEventHandler, AS.Application.Handlers.EventHandler>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();

var eventHandler = builder.Services.BuildServiceProvider().GetService<IEventHandler>();
var dispatcher = new Dispatcher<BaseEvent>();
dispatcher.RegisterNoContentHandler<SendBookingEvent>(eventHandler.Handle);
builder.Services.AddSingleton<IDispatcher>(dispatcher);

var host = builder.Build();
host.Run();
