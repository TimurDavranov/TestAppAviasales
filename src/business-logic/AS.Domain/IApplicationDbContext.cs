using AS.Domain.Entities.Aviasales;
using AS.Domain.Entities.Externals.Amadeus;
using AS.Domain.Entities.References;
using Microsoft.EntityFrameworkCore;

namespace AS.Domain;

public interface IApplicationDbContext
{
    DbSet<Booking> Bookings { get; set; }
    DbSet<Contract> Contracts { get; set; }
    DbSet<AmadeusTicket> AmadeusTickets { get; set; }
    DbSet<AmadeusTicketDestination> AmadeusTicketDestinations { get; set; }
    DbSet<Country> Country { get; set; }
    DbSet<State> States { get; set; }
    DbSet<City> City { get; set; }
    DbSet<Airport> Airports { get; set; }
    DbSet<AviaCompany> AviaCompanies { get; set; }
}
