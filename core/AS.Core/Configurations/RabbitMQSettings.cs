namespace AS.Core.Configurations;

public class RabbitMQSettings
{
    public string Connection { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public int? Port { get; set; }
    public string VirtualHost { get; set; }
    public string Url { get; set; }
}