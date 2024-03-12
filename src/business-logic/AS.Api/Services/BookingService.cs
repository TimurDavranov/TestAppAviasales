using AS.Api.Models;
using AS.Application.Dtos;
using AS.Application.Repositories;
using AS.Core.Constants;
using AS.Core.Events;
using AS.Core.Helpers;

namespace AS.Api.Services
{
    public interface IBookingService
    {
        Guid BookingCommand(BookingForm form, Guid userId);
        void BuyTicketCommand(Guid id, Guid ticketId);
        Task<BookingInfoResponse[]> GetUserBookings(Guid userId);
    }

    public class BookingService(IRabbitMessageProducer _rabbitMessageProducer, IBookingRepository _bookingRepository) : IBookingService
    {
        public Guid BookingCommand(BookingForm form, Guid userId)
        {
            var @event = new SendBookingEvent()
            {
                Id = Guid.NewGuid(),
                TicketId = Guid.NewGuid(),
                UserId = userId
            };

            _rabbitMessageProducer.Publish(ApplicationConstants.ASExchangeKey, ApplicationConstants.BookingRouteKey, ApplicationConstants.BookingQueueKey, @event);

            return @event.Id;
        }

        public void BuyTicketCommand(Guid id, Guid ticketId)
        {
            var @event = new BuyTicketEvent()
            {
                Id = id,
                TicketId = ticketId
            };

            _rabbitMessageProducer.Publish(ApplicationConstants.ASExchangeKey, ApplicationConstants.BookingRouteKey, ApplicationConstants.BookingQueueKey, @event);
        }

        public Task<BookingInfoResponse[]> GetUserBookings(Guid userId)
        {
            return _bookingRepository.GetUserBookingsInfo(userId);
        }
    }
}
