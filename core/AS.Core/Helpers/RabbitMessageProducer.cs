using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AS.Core.Helpers
{
    public interface IRabbitMessageProducer
    {
        void Publish<T>(string exchange, string route, string queue, T message);
    }

    public class RabbitMessageProducer : IRabbitMessageProducer
    {
        private bool _isConnected;
        private readonly IModel _channel;
        public RabbitMessageProducer(IRabbitConnection _rabbitConnection)
        {
            (_isConnected, _channel) = _rabbitConnection.Connect();
        }

        public void Publish<T>(string exchange, string route, string queue, T message)
        {
            if (_isConnected)
            {
                _channel.ExchangeDeclare(exchange, ExchangeType.Direct);
                if (message is null)
                    throw new ArgumentNullException(nameof(message), "Message must have a value!");

                var json = JsonSerializer.Serialize(message, message.GetType());
                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.QueueDeclare(queue, true, false, true);
                _channel.QueueBind(queue, exchange, route);

                _channel.BasicPublish(exchange, route, properties, body);

                _channel.Dispose();
            }
            else throw new Exception("Connection to RabbitMQ service is closed");
        }
    }
}
