using Microsoft.EntityFrameworkCore;

namespace AS.Core.Factories
{
    public sealed class DatabaseContextFactory<T> where T : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;
        public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public T CreateContext()
        {
            DbContextOptionsBuilder<T> optionsBuilder = new();
            _configureDbContext(optionsBuilder);

            Type anon = typeof(T);
            var instance = (T)Activator.CreateInstance(anon, [optionsBuilder.Options])!;
            return instance;
        }
    }
}
