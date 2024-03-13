using AS.Core.Primitives;

namespace AS.Application;

public static class QueryConst
{
    public static string GetUserBookingsInfoQuery =>
        "(select b.Id as \"Id\", b.RequestedDate as \"RequestedDate\", b.ExpiresDate as \"ExpiresDate\", at.Id as \"TicketId\", (countries.Name + ' ' + cities.Name + ' ' + airports.Name) as \"Title\", 0 as \"Source\"" +
        " from aviasales.bookings b" +
        " inner join externals.amadeus_tickets at on at.Id = b.TicketId" +
        " inner join [references].countries on countries.ISOCode3 = at.CountryCode" +
        " inner join [references].cities on cities.Id = at.CityId" +
        " inner join [references].airports on airports.Code = at.AirportCode" +
        " where b.UserId = @UserId)" +
        " union all" +
        " (select b.Id as \"Id\", b.RequestedDate as \"RequestedDate\", b.ExpiresDate as \"ExpiresDate\", at.Id as \"TicketId\", (countries.Name + ' ' + cities.Name + ' ' + airports.Name) as \"Title\", 1 as \"Source\"" +
        " from aviasales.bookings b" +
        " inner join externals.skyscanner_tickets at on at.Id = b.TicketId" +
        " inner join [references].countries on countries.ISOCode3 = at.CountryCode" +
        " inner join [references].cities on cities.Id = at.CityId" +
        " inner join [references].airports on airports.Code = at.AirportCode" +
        " where b.UserId = @UserId)";

    public static string FilterTicketsQuery(FilterModel filter)
    {
        var skip = (filter.Page - 1) * filter.Size;
        return $"select * from FilterTickets ('{filter.SearchText}', '{filter.SearchText}', 0, {skip}, {filter.Size})";
    }
}