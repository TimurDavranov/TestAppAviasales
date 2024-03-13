using AS.Core.Configurations;
using AS.Core.Primitives;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Identity.Data;
using System.Net;

namespace AS.UI.Services
{
    public class AccountService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ApplicationSettings _applicationSettings;
        private readonly ILocalStorageService _localStorageService;
        public AccountService(IHttpClientFactory factory, ApplicationSettings applicationSettings, ILocalStorageService localStorageService)
        {
            _factory = factory;
            _applicationSettings = applicationSettings;
            _localStorageService = localStorageService;
        }

        public async Task<AccessTokenResponse> Login(LoginRequest model)
        {
            using var client = _factory.CreateClient();

            client.BaseAddress = new Uri(_applicationSettings.BackendEndpoint);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serialized = JsonSerializer.Serialize(model);

            var content = new StringContent(serialized, Encoding.UTF8, "application/json");

            var request = await client.PostAsync("/login", content);

            if (request.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"{request.ReasonPhrase} {request.RequestMessage?.RequestUri}");
            }

            if (!request.IsSuccessStatusCode)
                throw new Exception(request.ReasonPhrase);

            var response = await request.Content.ReadFromJsonAsync<AccessTokenResponse>(BaseApiClient._options);

            return response;
        }

        public async Task<InfoResponse> Info()
        {
            var _token = await _localStorageService.GetItemAsync<string>("token");

            using var client = _factory.CreateClient();

            client.BaseAddress = new Uri(_applicationSettings.BackendEndpoint);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var request = await client.GetAsync("/info");

            if (request.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"{request.ReasonPhrase} {request.RequestMessage?.RequestUri}");
            }

            if (!request.IsSuccessStatusCode)
                throw new Exception(request.ReasonPhrase);

            var response = await request.Content.ReadFromJsonAsync<InfoResponse>(BaseApiClient._options);

            return response;
        }
    }
}
