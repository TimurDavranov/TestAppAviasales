using AS.Core.Configurations;
using AS.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AS.Migrator
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly string _connectionString;
        public ApplicationDbContextFactory()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            _connectionString = configuration[$"{nameof(ApplicationSettings)}:{nameof(ApplicationSettings.DBConnectionString)}"] ?? throw new ArgumentNullException(nameof(ApplicationSettings.DBConnectionString));
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(_connectionString, b => b.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
