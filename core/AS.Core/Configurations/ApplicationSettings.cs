namespace AS.Core.Configurations
{
    public class ApplicationSettings
    {
        public string DBConnectionString { get; set; }

        public string BackendEndpoint { get; set; }

        public InternalServiceSettings InternalServiceSettings { get; set; }

        public RabbitMQSettings RabbitMQSettings { get; set; }

        public RedisSettings RedisSettings { get; set; }
    }
}