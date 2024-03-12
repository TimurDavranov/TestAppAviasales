using AS.Api.Services;
using AS.Application.Dtos;
using AS.Core.Primitives;
using AS.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AS.Api.Controllers
{
    public class QueryController(IHttpContextAccessor _httpContextAccessor, IBookingService _bookingService, ITicketService _ticketService, RedisService _redisService)
        : BaseApiController(_httpContextAccessor)
    {
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<BookingInfoResponse[]>))]
        public async Task<IActionResult> GetUserBookings()
        {
            var results = _redisService.GetValue<BookingInfoResponse[]>(UserId.ToString());

            if (results is null)
            {
                results = await _bookingService.GetUserBookingsQuery(UserId);

                _redisService.SetValue(UserId.ToString(), results, TimeSpan.FromMinutes(1));
            }

            return Ok(results);
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<PagginationResult<TicketResponse[]>>))]
        public async Task<IActionResult> FilterTicket([FromBody] FilterModel filter)
        {
            var results = _redisService.GetValue<PagginationResult<TicketResponse[]>>(FilterModel.ToRequestKey(filter));

            if (results is null)
            {
                results = await _ticketService.FilterTickets(filter);

                _redisService.SetValue(FilterModel.ToRequestKey(filter), results, TimeSpan.FromMinutes(1));
            }
            
            return Ok(results);
        }
    }
}