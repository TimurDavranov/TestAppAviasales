using AS.Domain.Entities.Aviasales;
using AS.Domain.Entities.Externals.Amadeus;
using AS.Domain.Entities.Externals.Skyscanner;
using AS.Domain.Entities.References;
using Microsoft.EntityFrameworkCore;

namespace AS.Domain;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    // Aviasales scheme
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    // External scheme
    public DbSet<AmadeusTicket> AmadeusTickets { get; set; }
    public DbSet<AmadeusTicketDestination> AmadeusTicketDestinations { get; set; }
    public DbSet<SkyscannerTicket> SkyscannerTickets { get; set; }
    public DbSet<SkyscannerTicketDestination> SkyscannerTicketDestinations { get; set; }

    // References scheme
    public DbSet<Country> Country { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<City> City { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<AviaCompany> AviaCompanies { get; set; }
}
