using AS.Api.Services;
using AS.Application.Dtos;
using AS.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AS.Api.Controllers
{
    public class QueryController(IHttpContextAccessor _httpContextAccessor, IBookingService _bookingService) : BaseApiController(_httpContextAccessor)
    {
        [HttpGet("{id}")]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<BookingInfoResponse[]>))]
        public async Task<IActionResult> GetUserBookings()
        {
            var result = await _bookingService.GetUserBookings(UserId);

            return Ok(result);
        }

        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<PagginationResult<object>>))]
        public async Task<IActionResult> FilterTicket(FilterModel filter)
        {

            return Ok();
        }
    }
}
