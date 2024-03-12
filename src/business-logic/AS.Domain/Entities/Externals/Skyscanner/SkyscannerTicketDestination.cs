using AS.Core.Primitives;
using AS.Domain.Entities.Externals.Amadeus;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.Externals.Skyscanner
{
    [Table("skyscanner_ticket_destinations", Schema = "externals")]
    public class SkyscannerTicketDestination : BaseEntity<Guid>
    {
        public int Order { get; set; }
        public string CountryCode { get; set; }
        public Guid CityId { get; set; }
        public string AirportCode { get; set; }
        public DateTime DestinationDate { get; set; }
        public AmadeusTicket AmadeusTicket { get; set; }
    }
}
