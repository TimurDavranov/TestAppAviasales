using AS.Application.Dtos;
using AS.Core.Configurations;
using AS.Core.Primitives;
using AS.UI.Data;
using Blazored.LocalStorage;

namespace AS.UI.Services
{
    public class BackendService : BaseApiClient
    {
        private readonly ILocalStorageService _localStorageService;
        public BackendService(IHttpClientFactory factory, IHttpContextAccessor httpContext, ILocalStorageService localStorageService, ApplicationSettings applicationSettings) : base(applicationSettings.BackendEndpoint, factory, httpContext)
        {
            _localStorageService = localStorageService;
        }

        public async Task<string> Booking(BookingForm form)
        {
            var _token = await _localStorageService.GetItemAsync<string>("token");
            return await Post<string>("api/command/booking", form, _token);
        }

        public async Task<string> BuyTicket(string id, BookingForm form)
        {
            var _token = await _localStorageService.GetItemAsync<string>("token");
            return await Post<string>($"api/command/buyticketcommand/{id}", form, _token);
        }

        public async Task<BookingInfoResponse[]> GetUserBookings()
        {
            var _token = await _localStorageService.GetItemAsync<string>("token");
            return await Get<BookingInfoResponse[]>($"api/query/getuserbookings", _token);
        }

        public async Task<PagginationResult<TicketResponse[]>> FilterTicket(FilterModel filter)
        {
            var _token = await _localStorageService.GetItemAsync<string>("token");
            return await Post<PagginationResult<TicketResponse[]>>($"api/query/filterticket", filter, _token);
        }
    }
}
