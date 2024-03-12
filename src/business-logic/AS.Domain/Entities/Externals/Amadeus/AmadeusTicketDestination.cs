using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.Externals.Amadeus
{
    [Table("amadeus_ticket_destinations", Schema = "externals")]
    public class AmadeusTicketDestination : BaseEntity<Guid>
    {
        public int Order { get; set; }
        public string CountryCode { get; set; }
        public Guid CityId { get; set; }
        public string AirportCode { get; set; }
        public DateTime DestinationDate { get; set; }
        public AmadeusTicket AmadeusTicket { get; set; }
    }
}
