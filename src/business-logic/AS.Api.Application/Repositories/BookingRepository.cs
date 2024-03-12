using AS.Core.Configurations;
using AS.Core.Factories;
using AS.Domain;
using AS.Domain.Entities.Aviasales;
using Microsoft.EntityFrameworkCore;

namespace AS.Application.Repositories
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking model);
        Task<Booking?> Get(Guid Id);
        Task UpdateAsync(Booking model);
    }

    public sealed class BookingRepository(DatabaseContextFactory<ApplicationDbContext> _contextFactory, ApplicationSettings _applicationSettings) : IBookingRepository
    {
        public async Task<Guid> CreateAsync(Booking model)
        {
            using var context = _contextFactory.CreateContext();
            
            await context.AddAsync(model);
            await context.SaveChangesAsync();
            
            return model.Id;
        }

        public Task<Booking?> Get(Guid Id)
        {
            using var context = _contextFactory.CreateContext();
            return context.Bookings.FirstOrDefaultAsync(s => s.Id == Id);
        }

        public async Task UpdateAsync(Booking model)
        {
            using var context = _contextFactory.CreateContext();

            context.Update(model);
            await context.SaveChangesAsync();
        }
    }
}
