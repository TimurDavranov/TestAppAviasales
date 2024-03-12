namespace AS.Application.Dtos
{
    public class BookingInfoResponse
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public Guid TicketId { get; set; }
    }

    public class Booking
}
