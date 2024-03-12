using AS.Core.Enums;

namespace AS.Core.Events
{
    public class SendBookingEvent : BaseEvent
    {
        public SendBookingEvent() : base(nameof(SendBookingEvent))
        {
        }

        public Guid UserId { get; set; }
        public Guid TicketId { get; set; }
        public TicketSource Source { get; set; }
    }

    public class BuyTicketEvent : BaseEvent
    {
        public BuyTicketEvent() : base(nameof(BuyTicketEvent))
        {
        }

        public Guid TicketId { get; set; }
    }
}
