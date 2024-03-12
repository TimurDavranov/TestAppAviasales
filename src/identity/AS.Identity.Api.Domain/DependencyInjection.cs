using AS.Core.Configurations;
using AS.Core.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AS.Identity.Api.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureDI(this IServiceCollection services, ApplicationSettings appSettings)
        {
            Action<DbContextOptionsBuilder> dbOptions =
                opt => opt
                        .UseSqlServer(appSettings.DBConnectionString,
                            o => o.EnableRetryOnFailure())
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();

            services
                        .AddDbContext<IApplicationDbContext, ApplicationDbContext>(dbOptions, ServiceLifetime.Scoped);
            services.AddSingleton(new DatabaseContextFactory<ApplicationDbContext>(dbOptions));

            return services;
        }
    }
}
