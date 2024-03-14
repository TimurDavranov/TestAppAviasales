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
        /// <summary>
        /// Получение всех запросов пользователя на бронирование авиабилетов
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<BookingInfoResponse[]>))]
        public async Task<IActionResult> GetUserBookings()
        {
            var results = _redisService.GetValue<BookingInfoResponse[]>(UserId.ToString());

            if (results is null)
            {
                results = await _bookingService.GetUserBookingsQuery(UserId);

                _redisService.SetValue(UserId.ToString(), results, TimeSpan.FromSeconds(5));
            }

            return Ok(results);
        }

        /// <summary>
        /// Получение авиабилетов по определенному фильтру
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<PagginationResult<TicketResponse[]>>))]
        public async Task<IActionResult> FilterTicket([FromBody] FilterModel filter)
        {
            var results = _redisService.GetValue<PagginationResult<TicketResponse[]>>(FilterModel.ToRequestKey(filter));

            if (results is null)
            {
                results = await _ticketService.FilterTickets(filter);

                _redisService.SetValue(FilterModel.ToRequestKey(filter), results, TimeSpan.FromSeconds(5));
            }
            
            return Ok(results);
        }
    }
}