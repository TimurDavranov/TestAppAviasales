using AS.Api.Models;
using AS.Application;
using AS.Application.Dtos;
using AS.Core.Configurations;
using AS.Core.Constants;
using AS.Core.Events;
using AS.Core.Helpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AS.Api.Services
{
    public interface IBookingService
    {
        Guid BookingCommand(BookingForm form, Guid userId);
        void BuyTicketCommand(Guid id, Guid ticketId);
        Task<BookingInfoResponse[]> GetUserBookingsQuery(Guid userId);
    }

    public class BookingService(IRabbitMessageProducer _rabbitMessageProducer, ApplicationSettings _applicationSettings)
        : IBookingService
    {
        public Guid BookingCommand(BookingForm form, Guid userId)
        {
            var @event = new SendBookingEvent()
            {
                Id = Guid.NewGuid(),
                TicketId = form.TicketId,
                UserId = userId,
                Source = form.Source
            };

            _rabbitMessageProducer.Publish(ApplicationConstants.ASExchangeKey, ApplicationConstants.BookingRouteKey,
                ApplicationConstants.BookingQueueKey, @event);

            return @event.Id;
        }

        public void BuyTicketCommand(Guid id, Guid ticketId)
        {
            var @event = new BuyTicketEvent()
            {
                Id = id,
                TicketId = ticketId
            };

            _rabbitMessageProducer.Publish(ApplicationConstants.ASExchangeKey, ApplicationConstants.BookingRouteKey,
                ApplicationConstants.BookingQueueKey, @event);
        }

        public async Task<BookingInfoResponse[]> GetUserBookingsQuery(Guid userId)
        {
            using var connection = new SqlConnection(_applicationSettings.DBConnectionString);
            await connection.OpenAsync();
            var results = await connection.QueryAsync<BookingInfoResponse>(QueryConst.GetUserBookingsInfoQuery,
                new { UserId = userId.ToString() });
            await connection.CloseAsync();

            return results.ToArray();
        }
    }
}