using AS.Application.Repositories;
using AS.Core.Events;
using AS.Domain.Entities.Aviasales;

namespace AS.Application.Handlers
{
    public interface IEventHandler
    {
        Task Handle(SendBookingEvent @event);
    }

    public class EventHandler(IBookingRepository _bookingRepository) : IEventHandler
    {
        public Task Handle(SendBookingEvent @event)
        {
            var category = new Booking()
            {
                Id = @event.Id,
                RequestedDate = DateTime.Now,
                ExpiresDate = DateTime.Now.AddMinutes(10),
                UserId = @event.UserId,
                Status = Core.Enums.BookingStatus.New,
                TicketId = @event.TicketId
            };
            
            return _bookingRepository.CreateAsync(category);
        }
    }
}
