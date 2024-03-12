using AS.Core.Enums;

namespace AS.Api.Models
{
    public class BookingForm
    {
        public Guid TicketId { get; set; }
        public TicketSource Source { get; set; }
    }
}
