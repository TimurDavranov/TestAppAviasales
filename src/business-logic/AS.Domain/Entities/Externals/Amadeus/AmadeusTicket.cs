using AS.Core.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.Domain.Entities.Externals.Amadeus
{
    [Table("amadeus_tickets", Schema = "externals")]
    public class AmadeusTicket : BaseEntity<Guid>
    {
        public string CountryCode { get; set; }
        public Guid CityId { get; set; }
        public string AirportCode { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Price { get; set; }
        public int NumberOfSeats { get; set; }
        public Guid AviaCompanyId { get; set; }
        public ICollection<AmadeusTicketDestination> Destinations { get; set; }
    }
}
