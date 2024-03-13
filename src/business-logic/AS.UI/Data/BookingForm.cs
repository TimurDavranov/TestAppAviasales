using AS.Core.Enums;

namespace AS.UI.Data
{
    public class BookingForm
    {
        public Guid TicketId { get; set; }

        public TicketSource Source { get; set; }
    }
}
