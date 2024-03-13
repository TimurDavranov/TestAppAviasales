using AS.Api.Models;
using AS.Api.Services;
using AS.Application;
using AS.Application.Dtos;
using AS.Core.Configurations;
using AS.Core.Constants;
using AS.Core.Enums;
using AS.Core.Events;
using AS.Core.Helpers;
using Moq;
using SA.UnitTests.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.UnitTests.Services
{
    public class BookingServiceTest
    {
        private readonly ApplicationSettings _appSettings = new ApplicationSettings
        {
            RabbitMQSettings = new RabbitMQSettings
            {
                Connection = "109.123.245.64",
                User = "admin",
                Password = "hN27f27fwZPsjFYqv7a9",
                Port = 5672,
                VirtualHost = "/",
                Url = ""
            },
            DBConnectionString = "Server=109.123.245.64,1433;Initial Catalog=aviasales;User ID=sa;Persist Security Info=False;Password=qO8PjbG9tikr13SbOnaU;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;"
        };

        [Fact]
        public async Task GetUserBookingsQuery_ReturnsBookingInfoArray()
        {
            
            var userId = Guid.NewGuid();
            
            var mockMessageProducer = new Mock<IRabbitMessageProducer>();
            var yourClassInstance = new BookingService(mockMessageProducer.Object, _appSettings); 

            
            var result = await yourClassInstance.GetUserBookingsQuery(userId);

            
            Assert.IsType<BookingInfoResponse[]>(result); 
        }

        [Fact]
        public void BookingCommand_PublishesCorrectEvent()
        {
            var form = new BookingForm
            {
                TicketId = new Guid("24d5eb21-27e8-42dc-a226-08dc42b5cb8c"),
                Source = TicketSource.Amadeus
            };

            var userId = Guid.NewGuid();

            var mockMessageProducer = new Mock<IRabbitMessageProducer>();

            var yourClassInstance = new BookingService(mockMessageProducer.Object, _appSettings);


            var result = yourClassInstance.BookingCommand(form, userId);

            Assert.NotEqual(Guid.Empty, result);
            mockMessageProducer.Verify(
                x => x.Publish(
                    ApplicationConstants.ASExchangeKey,
                    ApplicationConstants.BookingRouteKey,
                    ApplicationConstants.BookingQueueKey,
                    It.Is<SendBookingEvent>(e =>
                        e.TicketId == form.TicketId &&
                        e.UserId == userId &&
                        e.Source == form.Source)),
                Times.Once);
        }

        [Fact]
        public void BuyTicketCommand_PublishesCorrectEvent()
        {
            var id = new Guid("adb7d145-ade0-4a7f-9c46-d36ddab42369");

            var ticketId = new Guid("24d5eb21-27e8-42dc-a226-08dc42b5cb8c");

            var mockMessageProducer = new Mock<IRabbitMessageProducer>();

            var yourClassInstance = new BookingService(mockMessageProducer.Object, _appSettings);

            yourClassInstance.BuyTicketCommand(id, ticketId);

            mockMessageProducer.Verify(
                x => x.Publish(
                    ApplicationConstants.ASExchangeKey,
                    ApplicationConstants.BookingRouteKey,
                    ApplicationConstants.BookingQueueKey,
                    It.Is<BuyTicketEvent>(e =>
                        e.Id == id && e.TicketId == ticketId)),
                Times.Once);
        }
    }
}
