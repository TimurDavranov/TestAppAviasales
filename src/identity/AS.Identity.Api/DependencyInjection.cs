using AS.Core.Configurations;
using AS.Core.Factories;
using AS.Identity.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace AS.Identity.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureDI(this IServiceCollection services, ApplicationSettings settings)
        {
            Action<DbContextOptionsBuilder> dbOptions =
                opt => opt
                        .UseSqlServer(settings.DBConnectionString,
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
