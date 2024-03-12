using AS.Core.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AS.Core.Helpers
{
    public interface IRabbitMessageConsumer
    {
        void Consume(string exchangeKey, string routeKey, string queueKey, Action<object?, BasicDeliverEventArgs, IModel> action, uint prefetchSize, ushort prefetchCount);
        void Consume(string exchangeKey, string routeKey, string queueKey, Func<object?, BasicDeliverEventArgs, IModel, Task> action, uint prefetchSize, ushort prefetchCount);
    }

    public sealed class RabbitMessageConsumer : IRabbitMessageConsumer
    {
        private readonly bool _isConnected;
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly IConnectionFactory _factory;

        public RabbitMessageConsumer(IRabbitConnection _rabbitConnection)
        {
            (_isConnected, _channel) = _rabbitConnection.Connect();
        }

        public void Consume(string exchangeKey, string routeKey, string queueKey, Action<object?, BasicDeliverEventArgs, IModel> action, uint prefetchSize, ushort prefetchCount)
        {
            if (_isConnected)
            {
                _channel.ExchangeDeclare(exchangeKey, ExchangeType.Direct);

                _channel.QueueDeclare(queueKey, true, false, true);

                _channel.QueueBind(queueKey, exchangeKey, routeKey);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += (sender, args) =>
                {
                    action.Invoke(sender, args, _channel);
                };
                _channel.BasicQos(prefetchSize: prefetchSize, prefetchCount: prefetchCount, false);
                _channel.BasicConsume(queueKey, false, consumer);
            }
            else throw new Exception("Connection to RabbitMQ service is closed");
        }

        public void Consume(string exchangeKey, string routeKey, string queueKey, Func<object?, BasicDeliverEventArgs, IModel, Task> action, uint prefetchSize, ushort prefetchCount)
        {
            if (_isConnected)
            {
                _channel.ExchangeDeclare(exchangeKey, ExchangeType.Direct);

                var queueName = _channel.QueueDeclare(queueKey, true, false, true).QueueName;

                _channel.QueueBind(queueName, exchangeKey, routeKey);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (sender, args) =>
                {
                    await action.Invoke(sender, args, _channel);
                    _channel.BasicAck(args.DeliveryTag, true);
                };
                _channel.BasicQos(prefetchSize: prefetchSize, prefetchCount: prefetchCount, false);
                _channel.BasicConsume(queueName, false, consumer);
            }
            else throw new Exception("Connection to RabbitMQ service is closed");
        }
    }
}
