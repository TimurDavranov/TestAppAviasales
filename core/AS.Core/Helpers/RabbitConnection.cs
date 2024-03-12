using AS.Core.Configurations;
using RabbitMQ.Client;

namespace AS.Core.Helpers
{
    public interface IRabbitConnection
    {
        (bool, IModel) Connect();
    }

    public class RabbitConnection(ApplicationSettings _applicationSettings) : IRabbitConnection
    {
        public (bool, IModel) Connect()
        {
            bool _isConnected;
            IModel _channel;
            IConnection _connection;
            IConnectionFactory _factory;

            if (_applicationSettings?.RabbitMQSettings is null)
                throw new ArgumentNullException(nameof(ApplicationSettings.RabbitMQSettings), "RabbitMQ settings is empty");

            if (string.IsNullOrEmpty(_applicationSettings.RabbitMQSettings.Url))
                _factory = new ConnectionFactory
                {
                    HostName = _applicationSettings.RabbitMQSettings.Connection,
                    Password = _applicationSettings.RabbitMQSettings.Password,
                    UserName = _applicationSettings.RabbitMQSettings.User,
                    Port = _applicationSettings.RabbitMQSettings.Port ?? 5672,
                    VirtualHost = _applicationSettings.RabbitMQSettings.VirtualHost,
                };
            else
                _factory = new ConnectionFactory
                {
                    Uri = new Uri(_applicationSettings.RabbitMQSettings.Url)
                };

            _connection = _factory.CreateConnection();

            _channel = _connection.CreateModel();

            if (_connection.IsOpen) _isConnected = true;
            else throw new Exception("Can't connect to RabbitMQ service");

            return (_isConnected, _channel);
        }
    }
}
