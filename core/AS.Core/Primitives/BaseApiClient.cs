using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace AS.Core.Primitives
{
    public abstract class BaseApiClient
    {
        protected string _baseUrl { get; }
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            MaxDepth = int.MaxValue,
            Converters =
            {
                new DateTimeConverter()
            }
        };

        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _httpContext;

        protected BaseApiClient(string baseUrl, IHttpClientFactory factory, IHttpContextAccessor httpContext)
        {
            _baseUrl = baseUrl;
            _factory = factory;
            _httpContext = httpContext;
        }

        protected HttpClient CreateHttpClient()
        {
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            return client;
        }

        protected virtual async Task<T> Get<T>(string routeUrl) where T : class
        {
            var client = CreateHttpClient();

            var request = await client.GetAsync(routeUrl);

            return await ConvertResponse<T>(request);
        }

        protected virtual async Task<T> GetSimple<T>(string routeUrl) where T : class
        {
            var client = CreateHttpClient();

            var request = await client.GetAsync(routeUrl);

            return await ConvertResponseSimple<T>(request);
        }

        protected virtual async Task<T> Post<T>(string routeUrl, object bodyData) where T : class
        {
            var client = CreateHttpClient();

            var serialized = JsonSerializer.Serialize(bodyData);
            var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(routeUrl, content);
            return await ConvertResponse<T>(request);
        }

        protected virtual async Task<T> PostSimple<T>(string routeUrl, object bodyData) where T : class
        {
            var client = CreateHttpClient();

            var serialized = JsonSerializer.Serialize(bodyData);
            var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            var request = await client.PostAsync(routeUrl, content);
            return await ConvertResponseSimple<T>(request);
        }

        protected virtual async Task Put<T>(string routeUrl, object bodyData) where T : class
        {
            var client = CreateHttpClient();
            var serialized = JsonSerializer.Serialize(bodyData);

            var content = new StringContent(serialized, Encoding.UTF8, "application/json");
            var request = await client.PutAsync(routeUrl, content);

            await ConvertResponse<T>(request);
        }

        protected virtual async Task Delete(string routeUrl)
        {
            var client = CreateHttpClient();

            var request = await client.DeleteAsync(routeUrl);

            await ConvertResponse(request);
        }

        private static async Task<T> ConvertResponse<T>(HttpResponseMessage request) where T : class
        {
            try
            {
                if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"{request.ReasonPhrase} {request.RequestMessage?.RequestUri}");
                }

                if (!request.IsSuccessStatusCode)
                    throw new Exception(request.ReasonPhrase);

                var response = await request.Content.ReadFromJsonAsync<BaseApiResponse<T>>(_options);

                if (!response.Success)
                    throw new Exception(response.Message);

                return response.Data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task<T> ConvertResponseSimple<T>(HttpResponseMessage request) where T : class
        {
            try
            {
                if (request.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"{request.ReasonPhrase} {request.RequestMessage?.RequestUri}");
                }

                if (!request.IsSuccessStatusCode)
                    throw new Exception(request.ReasonPhrase);

                var response = await request.Content.ReadFromJsonAsync<T>(_options);

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static async Task<object?> ConvertResponse(HttpResponseMessage request)
        {
            if (!request.IsSuccessStatusCode)
                throw new Exception(request.ReasonPhrase);

            var response = await request.Content.ReadFromJsonAsync<BaseApiResponse<object>>(_options);

            if (!response.Success)
                throw new Exception(response.Message);

            return response.Data;
        }

        private string GetToken()
        {
            var authorizationHeader = _httpContext.HttpContext.Request.Headers["Authorization"];
            string accessToken = string.Empty;
            if (authorizationHeader.ToString().StartsWith("Bearer"))
            {
                accessToken = authorizationHeader.ToString().Substring("Bearer ".Length).Trim();
            }
            return accessToken;
        }
    }

    public class DateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        }
    }
}
