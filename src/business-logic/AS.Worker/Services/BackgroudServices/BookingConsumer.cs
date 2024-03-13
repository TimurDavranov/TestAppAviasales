using AS.Core.Constants;
using AS.Core.Converters;
using AS.Core.Events;
using AS.Core.Helpers;
using System.Text;
using System.Text.Json;

namespace AS.Worker.Services.BackgroudServices
{
    public class BookingConsumer(IServiceScopeFactory _serviceScopeFactory, IDispatcher _dispatcher) : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();

            var consumer = scope.ServiceProvider.GetRequiredService<IRabbitMessageConsumer>();

            consumer.Consume(ApplicationConstants.ASExchangeKey, ApplicationConstants.BookingRouteKey, ApplicationConstants.BookingQueueKey,
                async (sender, args, channel) =>
                    {
                        try
                        {
                            var body = Encoding.UTF8.GetString(args.Body.ToArray());
                            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                            var message = JsonSerializer.Deserialize<BaseEvent>(Encoding.UTF8.GetString(args.Body.ToArray()), options);

                            if (message is not null)
                                await _dispatcher.SendNoContentAsync(message);
                        }
                        catch(Exception ex) 
                        {
                            var asd = "";
                        }
                    },
                1, 1);

            return Task.CompletedTask;
        }
    }
}
