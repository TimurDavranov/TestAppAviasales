using AS.Core.Enums;

namespace AS.Application.Dtos
{
    public class BookingInfoResponse
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public Guid TicketId { get; set; }
        public TicketSource Source { get; set; }
        public BookingStatus Status { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
    }
}