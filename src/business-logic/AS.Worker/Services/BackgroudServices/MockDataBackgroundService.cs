using AS.Core.Factories;
using AS.Domain;
using AS.Domain.Entities.Externals.Amadeus;
using AS.Domain.Entities.Externals.Skyscanner;
using AS.Domain.Entities.References;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AS.Worker.Services.BackgroudServices
{
    public class MockDataBackgroundService(DatabaseContextFactory<ApplicationDbContext> _contextFactory) : BackgroundService
    {
        private JsonObject[] countryJObjects;
        private JsonObject[] airports;
        private string[] aviaCompanies;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ReadJson();
            //await AddReferences(stoppingToken);
            //await AddAviaCompanies(stoppingToken);
            await AddExternals(stoppingToken);
        }

        private void ReadJson()
        {
            using var countriesSR = new StreamReader("countries_states_cities.json");
            string countriesJson = countriesSR.ReadToEnd();
            countryJObjects = JsonSerializer.Deserialize<JsonObject[]>(countriesJson)!;

            using var airportsSR = new StreamReader("airports.json");
            string airportsJson = airportsSR.ReadToEnd();
            airports = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(airportsJson)!
                .Select(s => s.Value)
                .ToArray();

            using var aviaCompaniesSR = new StreamReader("aviacompany_names.json");
            string aviaCompaniesJson = aviaCompaniesSR.ReadToEnd();
            aviaCompanies = JsonSerializer.Deserialize<string[]>(aviaCompaniesJson)!
                .Where(s => !string.IsNullOrWhiteSpace(s) && s.Length > 3)
                .ToArray();
        }

        private async Task AddReferences(CancellationToken stoppingToken)
        {
            using var context = _contextFactory.CreateContext();

            foreach (var country in BuildReferences())
            {
                var countryEntity = await context.Country
                    .Include(s => s.States).ThenInclude(s => s.Cities).ThenInclude(s => s.Airports)
                    .FirstOrDefaultAsync(c => c.NumericCode == country.NumericCode, stoppingToken);

                if (countryEntity is not null)
                {
                    countryEntity.ISOCode2 = country.ISOCode2;
                    countryEntity.ISOCode3 = country.ISOCode3;
                    countryEntity.Name = country.Name;
                    countryEntity.NumericCode = country.NumericCode;

                    foreach (var state in country.States)
                    {
                        var stateEntity = countryEntity.States
                            .FirstOrDefault(s => s.Code.Equals(state.Code, StringComparison.OrdinalIgnoreCase));

                        if (stateEntity is not null)
                        {
                            stateEntity.Name = state.Name;
                            stateEntity.Code = state.Code;

                            foreach (var city in state.Cities)
                            {
                                var cityEntity = stateEntity.Cities
                                    .FirstOrDefault(s => s.Name.Equals(city.Name, StringComparison.OrdinalIgnoreCase));
                                if (cityEntity is not null)
                                {
                                    cityEntity.Name = city.Name;

                                    foreach (var airport in city.Airports)
                                    {
                                        var airportEntity = cityEntity.Airports
                                            .FirstOrDefault(s => s.Code.Equals(airport.Code, StringComparison.OrdinalIgnoreCase));
                                        if (airportEntity is not null)
                                        {
                                            airportEntity.Name = airport.Name;
                                            airportEntity.Code = airport.Code;
                                        }
                                        else
                                        {
                                            cityEntity.Airports.Add(airport);
                                        }
                                    }
                                }
                                else
                                {
                                    state.Cities.Add(city);
                                }
                            }
                        }
                        else
                        {
                            countryEntity.States.Add(state);
                        }
                    }
                }
                else
                {
                    await context.AddAsync(country, stoppingToken);
                }
            }

            if (context.ChangeTracker.Entries().Any(s => s.State is EntityState.Modified or EntityState.Added))
                await context.SaveChangesAsync(stoppingToken);
        }

        private async Task AddAviaCompanies(CancellationToken stoppingToken)
        {
            var context = _contextFactory.CreateContext();

            for (var index = 0; index < aviaCompanies.Length; index++)
            {
                var code = string.Join("", aviaCompanies[index].Split(" ").Select(s => s.Trim().FirstOrDefault())).ToUpper();

                if (await context.AviaCompanies.AnyAsync(s => s.Code.ToUpper() == code))
                    continue;

                await context.AviaCompanies.AddAsync(new AviaCompany
                {
                    Code = code,
                    Name = aviaCompanies[index]
                }, stoppingToken);
            }

            if (context.ChangeTracker.Entries().Any(s => s.State is EntityState.Modified or EntityState.Added))
                await context.SaveChangesAsync(stoppingToken);
        }

        private async Task AddExternals(CancellationToken stoppingToken)
        {
            using var context = _contextFactory.CreateContext();

            List<AmadeusTicket> amadeusTickets = [];
            List<SkyscannerTicket> skyscannerTickets = [];

            var countries = await context.Country
                .AsNoTracking()
                .Include(s => s.States).ThenInclude(s => s.Cities).ThenInclude(s => s.Airports)
                .ToArrayAsync(stoppingToken);

            var aviacompanies = await context.AviaCompanies
                .AsNoTracking()
                .ToArrayAsync();

            amadeusTickets.AddRange(await AddAmadeusTickets(context, countries, aviacompanies, stoppingToken));
            skyscannerTickets.AddRange(await AddSkyscannerTickets(context, countries, aviacompanies, stoppingToken));

            await context.AddRangeAsync(amadeusTickets, stoppingToken);
            await context.AddRangeAsync(skyscannerTickets, stoppingToken);
            await context.SaveChangesAsync(stoppingToken);
        }

        private async Task<ICollection<AmadeusTicket>> AddAmadeusTickets(ApplicationDbContext context, Country[] countries, AviaCompany[] aviaCompanies, CancellationToken stoppingToken)
        {
            ICollection<AmadeusTicket> tickets = [];

            var random = new Random();

            int duration = 10;

            int minTransferAmount = 0;

            int maxTransferAmount = 4;

            int amountTicketsInDay = 50;

            var startDate = DateTime.Today;

            for (var durationIndex = 0; durationIndex < duration; durationIndex++)
            {
                if (await context.AmadeusTickets.AnyAsync(s => s.DepartureDate.Date == startDate.AddDays(durationIndex), stoppingToken))
                {
                    duration++;

                    startDate = startDate.AddDays(durationIndex + 1);

                    continue;
                }

                var transferAmount = random.Next(minTransferAmount, maxTransferAmount);

                if (transferAmount > 0)
                {
                    for (int ticketAmountIndex = 0; ticketAmountIndex < amountTicketsInDay; ticketAmountIndex++)
                    {
                        var (country, city, airport) = GetRandomReferences(ref countries, ref random);
                        countries = countries.Where(s => s.Id != country.Id).ToArray();

                        var ticket = new AmadeusTicket
                        {
                            CountryCode = country.ISOCode3,
                            CityId = city.Id,
                            AirportCode = airport.Code,
                            DepartureDate = startDate
                                .AddHours(random.Next(0, 23))
                                .AddMinutes(random.Next(0, 59)),
                            Price = random.Next(transferAmount * 100, transferAmount * 200),
                            NumberOfSeats = random.Next(100, 400),
                            AviaCompanyId = aviaCompanies[random.Next(0, aviaCompanies.Length)].Id
                        };

                        var destinationReferences = GetSeveralRandomReferences(ref countries, ref random, ticketAmountIndex);

                        ticket.Destinations = [];
                        for (int transferAmountIndex = 0; transferAmountIndex < amountTicketsInDay; transferAmountIndex++)
                        {
                            var (destinationCountry, destinationCity, destinationAirport) = destinationReferences[transferAmountIndex];

                            var lastDestination = ticket.Destinations
                                .OrderByDescending(s => s.Order)
                                .FirstOrDefault();

                            ticket.Destinations.Add(new AmadeusTicketDestination
                            {
                                CountryCode = destinationCountry.ISOCode3,
                                CityId = destinationCity.Id,
                                AirportCode = destinationAirport.Code,
                                DestinationDate = startDate
                                    .AddHours(random.Next(lastDestination?.DestinationDate.Hour ?? ticket.DepartureDate.Hour, 23))
                                    .AddMinutes(random.Next(0, 59)),
                                Order = transferAmountIndex
                            });
                        }

                        tickets.Add(ticket);
                    }
                }
                else
                {
                    var (country, city, airport) = GetRandomReferences(ref countries, ref random);
                    var (destinationCountry, destinationCity, destinationAirport) = GetRandomReferences(ref countries, ref random);

                    var ticket = new AmadeusTicket
                    {
                        CountryCode = country.ISOCode3,
                        CityId = city.Id,
                        AirportCode = airport.Code,
                        DepartureDate = startDate
                                .AddHours(random.Next(0, 23))
                                .AddMinutes(random.Next(0, 59)),
                        Price = random.Next(transferAmount * 100, transferAmount * 200),
                        NumberOfSeats = random.Next(100, 400),
                        AviaCompanyId = aviaCompanies[random.Next(0, aviaCompanies.Length)].Id
                    };

                    ticket.Destinations = [
                            new AmadeusTicketDestination
                            {
                                CountryCode = destinationCountry.ISOCode3,
                                CityId = destinationCity.Id,
                                AirportCode = destinationAirport.Code,
                                DestinationDate = startDate
                                    .AddHours(random.Next(ticket.DepartureDate.Hour, 23))
                                    .AddMinutes(random.Next(0, 59)),
                                Order = 0
                            }];

                    tickets.Add(ticket);
                }

                startDate = startDate.AddDays(durationIndex + 1);
            }

            return tickets;
        }

        private async Task<ICollection<SkyscannerTicket>> AddSkyscannerTickets(ApplicationDbContext context, Country[] countries, AviaCompany[] aviaCompanies, CancellationToken stoppingToken)
        {
            ICollection<SkyscannerTicket> tickets = [];

            var random = new Random();

            int duration = 10;

            int minTransferAmount = 0;

            int maxTransferAmount = 4;

            int amountTicketsInDay = 50;

            var startDate = DateTime.Today;

            for (var durationIndex = 0; durationIndex < duration; durationIndex++)
            {
                if (await context.SkyscannerTickets.AnyAsync(s => s.DepartureDate.Date == startDate.AddDays(durationIndex), stoppingToken))
                {
                    duration++;

                    startDate = startDate.AddDays(durationIndex + 1);

                    continue;
                }

                var transferAmount = random.Next(minTransferAmount, maxTransferAmount);

                if (transferAmount > 0)
                {
                    for (int ticketAmountIndex = 0; ticketAmountIndex < amountTicketsInDay; ticketAmountIndex++)
                    {
                        var (country, city, airport) = GetRandomReferences(ref countries, ref random);
                        countries = countries.Where(s => s.Id != country.Id).ToArray();

                        var ticket = new SkyscannerTicket
                        {
                            CountryCode = country.ISOCode3,
                            CityId = city.Id,
                            AirportCode = airport.Code,
                            DepartureDate = startDate
                                .AddHours(random.Next(0, 23))
                                .AddMinutes(random.Next(0, 59)),
                            Price = random.Next(transferAmount * 100, transferAmount * 200),
                            NumberOfSeats = random.Next(100, 400),
                            AviaCompanyId = aviaCompanies[random.Next(0, aviaCompanies.Length)].Id
                        };

                        var destinationReferences = GetSeveralRandomReferences(ref countries, ref random, ticketAmountIndex);

                        ticket.Destinations = [];
                        for (int transferAmountIndex = 0; transferAmountIndex < amountTicketsInDay; transferAmountIndex++)
                        {
                            var (destinationCountry, destinationCity, destinationAirport) = destinationReferences[transferAmountIndex];

                            var lastDestination = ticket.Destinations
                                .OrderByDescending(s => s.Order)
                                .FirstOrDefault();

                            ticket.Destinations.Add(new SkyscannerTicketDestination
                            {
                                CountryCode = destinationCountry.ISOCode3,
                                CityId = destinationCity.Id,
                                AirportCode = destinationAirport.Code,
                                DestinationDate = startDate
                                    .AddHours(random.Next(lastDestination?.DestinationDate.Hour ?? ticket.DepartureDate.Hour, 23))
                                    .AddMinutes(random.Next(0, 59)),
                                Order = transferAmountIndex
                            });
                        }

                        tickets.Add(ticket);
                    }
                }
                else
                {
                    var (country, city, airport) = GetRandomReferences(ref countries, ref random);
                    var (destinationCountry, destinationCity, destinationAirport) = GetRandomReferences(ref countries, ref random);

                    var ticket = new SkyscannerTicket
                    {
                        CountryCode = country.ISOCode3,
                        CityId = city.Id,
                        AirportCode = airport.Code,
                        DepartureDate = startDate
                                .AddHours(random.Next(0, 23))
                                .AddMinutes(random.Next(0, 59)),
                        Price = random.Next(transferAmount * 100, transferAmount * 200),
                        NumberOfSeats = random.Next(100, 400),
                        AviaCompanyId = aviaCompanies[random.Next(0, aviaCompanies.Length)].Id
                    };

                    ticket.Destinations = [
                            new SkyscannerTicketDestination
                            {
                                CountryCode = destinationCountry.ISOCode3,
                                CityId = destinationCity.Id,
                                AirportCode = destinationAirport.Code,
                                DestinationDate = startDate
                                    .AddHours(random.Next(ticket.DepartureDate.Hour, 23))
                                    .AddMinutes(random.Next(0, 59)),
                                Order = 0
                            }];

                    tickets.Add(ticket);
                }

                startDate = startDate.AddDays(durationIndex + 1);
            }

            return tickets;
        }

        private (Country, City, Airport) GetRandomReferences(ref Country[] countries, ref Random random)
        {
            var country = countries[random.Next(0, countries.Count())];

            var states = country.States.ToArray();
            if (!states.Any())
                return GetRandomReferences(ref countries, ref random);

            var state = states[random.Next(0, states.Count())];
            if (state is null)
                return GetRandomReferences(ref countries, ref random);

            var cities = state.Cities.ToArray();
            if (!cities.Any())
                return GetRandomReferences(ref countries, ref random);

            var city = cities[random.Next(0, cities.Count())];
            if (city is null)
                return GetRandomReferences(ref countries, ref random);

            var airports = city.Airports.ToArray();
            if (!airports.Any())
                return GetRandomReferences(ref countries, ref random);

            var airport = airports[random.Next(0, airports.Count())];
            if (airport is null)
                return GetRandomReferences(ref countries, ref random);

            return new(country, city, airport);
        }

        private (Country, City, Airport)[] GetSeveralRandomReferences(ref Country[] countries, ref Random random, int count)
        {
            (Country, City, Airport)[] list = [];
            for (int i = 0; i < count; i++)
            {
                var result = GetRandomReferences(ref countries, ref random);

                while (list.Contains(result))
                    result = GetRandomReferences(ref countries, ref random);

                if (list.Length == count)
                    break;

                list = [.. list, result];
            };

            return list;
        }

        private IEnumerable<Country> BuildReferences()
        {
            foreach (var country in countryJObjects)
            {
                yield return new Country
                {
                    Name = country["name"].ToString(),
                    ISOCode2 = country["iso2"].ToString(),
                    ISOCode3 = country["iso3"].ToString(),
                    NumericCode = country["numeric_code"].ToString(),
                    States = country["states"]
                        .AsArray()
                        .Select(state => new State
                        {
                            Name = state["name"].ToString(),
                            Code = state["state_code"].ToString(),
                            Cities = state["cities"]
                                .AsArray()
                                .Select(city => new City
                                {
                                    Name = city["name"].ToString(),
                                    Airports = airports
                                    .Where(s =>
                                        s["country"].ToString().Equals(country["iso2"].ToString(), StringComparison.OrdinalIgnoreCase) &&
                                        s["state"].ToString().Equals(state["name"].ToString(), StringComparison.OrdinalIgnoreCase) &&
                                        s["city"].ToString().Equals(city["name"].ToString(), StringComparison.OrdinalIgnoreCase))
                                    .Select(s => new Airport
                                    {
                                        Code = s["icao"].ToString(),
                                        Name = s["name"].ToString()
                                    })
                                    .ToArray()
                                }).ToArray()
                        }).ToArray()
                };
            }
        }
    }
}
