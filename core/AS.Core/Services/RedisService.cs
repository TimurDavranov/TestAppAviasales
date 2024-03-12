using System.Text.Json;
using AS.Core.Configurations;
using StackExchange.Redis;

namespace AS.Core.Services;

public class RedisService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IDatabase _database;

    public RedisService(ApplicationSettings applicationSettings)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(applicationSettings.RedisSettings.Url);
        _database = _connectionMultiplexer.GetDatabase();
    }

    public T GetValue<T>(string key) where T : class
    {
        var value = _database.StringGet(key);
        if (value.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<T>(value);
    }

    public void SetValue<T>(string key, T value, TimeSpan timeSpan) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        _database.StringSet(key, json, timeSpan, When.Always);
    }
}