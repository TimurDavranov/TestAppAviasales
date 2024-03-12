using AS.Core.Enums;

namespace AS.Application.Dtos;

public class TicketResponse
{
    public int TotalCount { get; set; }
    public Guid TicketId { get; set; }
    public TicketSource Source { get; set; }
    public DateTime DepartureDate { get; set; }
    public string DeparturePlace { get; set; }
    public DateTime DestinationDate { get; set; }
    public string DestinationPlace { get; set; }
    public int TransferCount { get; set; }
    public decimal TicketPrice { get; set; }
    public int TicketCount { get; set; }
}