namespace AS.Application.Dtos
{
    public class BookingInfoResponse
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime ExpiresDate { get; set; }
        public Guid TicketId { get; set; }
        public DateTime DepartureDate { get; set; }
        public string DeparturePlace { get; set; }
        public DateTime DestinationDate { get; set; }
        public string DestinationPlace { get; set; }
        public int TransferCount { get; set; }
        public decimal TicketPrice { get; set; }
    }
}