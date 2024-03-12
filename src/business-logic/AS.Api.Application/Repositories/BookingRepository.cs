using AS.Application.Dtos;
using AS.Core.Factories;
using AS.Domain;
using AS.Domain.Entities.Aviasales;
using Microsoft.EntityFrameworkCore;

namespace AS.Application.Repositories
{
    public interface IBookingRepository
    {
        Task<Guid> CreateAsync(Booking model);
        Task<BookingInfoResponse[]> GetUserBookingsInfo(Guid userId);
    }

    public sealed class BookingRepository(DatabaseContextFactory<ApplicationDbContext> _contextFactory) : IBookingRepository
    {
        public async Task<Guid> CreateAsync(Booking model)
        {
            using var context = _contextFactory.CreateContext();
            
            await context.AddAsync(model);
            await context.SaveChangesAsync();
            
            return model.Id;
        }

        public async Task<BookingInfoResponse[]> GetUserBookingsInfo(Guid userId)
        {
            using var context = _contextFactory.CreateContext();

            var results = await context.Bookings
                .Where(s => s.UserId == userId)
                .AsSplitQuery()
                .AsNoTracking()
                .Select(s => new BookingInfoResponse
                {
                    RequestedDate = s.RequestedDate,
                    ExpiresDate = s.ExpiresDate,
                    Id = s.Id,
                    TicketId = s.TicketId,
                })
                .ToArrayAsync();

            return results;
        }
    }
}
