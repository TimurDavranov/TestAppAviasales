using AS.Application.Repositories;
using AS.Core.Enums;
using AS.Core.Events;
using AS.Domain.Entities.Aviasales;

namespace AS.Application.Handlers
{
    public interface IEventHandler
    {
        Task Handle(SendBookingEvent @event);
        Task Handle(BuyTicketEvent @event);
    }

    public class EventHandler(IBookingRepository _bookingRepository) : IEventHandler
    {
        public async Task Handle(SendBookingEvent @event)
        {
            var booking = new Booking()
            {
                Id = @event.Id,
                RequestedDate = DateTime.Now,
                ExpiresDate = DateTime.Now.AddMinutes(10),
                UserId = @event.UserId,
                Status = Core.Enums.BookingStatus.New,
                TicketId = @event.TicketId
            };

            await _bookingRepository!.CreateAsync(booking);
        }

        public async Task Handle(BuyTicketEvent @event)
        {
            var booking = await _bookingRepository!.Get(@event.Id);
            if (booking is null)
                return;

            if (booking.ExpiresDate < DateTime.Now)
            {
                booking.Status = BookingStatus.Failed;
                booking.Error = "Время бронирования билета истекло!";
                await _bookingRepository.UpdateAsync(booking);
                return;
            }

            booking.Status = BookingStatus.Paid;
            await _bookingRepository.UpdateAsync(booking);
        }
    }
}