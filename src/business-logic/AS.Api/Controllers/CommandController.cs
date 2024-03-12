using AS.Api.Models;
using AS.Api.Services;
using AS.Application.Dtos;
using AS.Core.Primitives;
using Microsoft.AspNetCore.Mvc;

namespace AS.Api.Controllers
{
    public class CommandController(IHttpContextAccessor _httpContextAccessor, IBookingService _bookingService) : BaseApiController(_httpContextAccessor)
    {
        [HttpPost]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<Guid>))]
        public IActionResult Booking([FromBody] BookingForm form)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Неправильные данные");
            }

            var result = _bookingService.BookingCommand(form, UserId);
            return Ok(result);
        }

        [HttpPost("{id}")]
        [ProducesDefaultResponseType(typeof(BaseApiResponse<object>))]
        public IActionResult BuyTicketCommand([FromBody] BookingForm form, [FromRoute] Guid id)
        {
            _bookingService.BuyTicketCommand(id, form.TicketId);

            return Ok();
        }
    }
}
