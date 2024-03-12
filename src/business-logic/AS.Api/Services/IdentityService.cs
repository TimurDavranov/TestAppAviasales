using AS.Core.Configurations;
using AS.Core.Primitives;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace AS.Infrastructure.Services
{
    public interface IIdentityService
    {
        Task<InfoResponse> Info();
        Task<AccessTokenResponse> Login(LoginRequest request);
    }

    public sealed class IdentityService : BaseApiClient, IIdentityService
    {
        public IdentityService(ApplicationSettings settings, IHttpClientFactory factory, IHttpContextAccessor httpContext) :
            base(settings.InternalServiceSettings.IdentityApi, factory, httpContext)
        {
        }

        public Task<AccessTokenResponse> Login(LoginRequest request)
        {
            return Post<AccessTokenResponse>("/login", request);
        }

        public Task<InfoResponse> Info()
        {
            return Get<InfoResponse>("/info");
        }
    }
}
