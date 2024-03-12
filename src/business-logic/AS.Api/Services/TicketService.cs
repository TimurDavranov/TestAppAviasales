using AS.Application;
using AS.Application.Dtos;
using AS.Core.Configurations;
using AS.Core.Primitives;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AS.Api.Services;

public interface ITicketService
{
    Task<PagginationResult<TicketResponse[]>> FilterTickets(FilterModel filter);
}

public class TicketService(ApplicationSettings _applicationSettings) : ITicketService
{
    public async Task<PagginationResult<TicketResponse[]>> FilterTickets(FilterModel filter)
    {
        using var connection = new SqlConnection(_applicationSettings.DBConnectionString);
        await connection.OpenAsync();
        var results = await connection.QueryAsync<TicketResponse>(QueryConst.FilterTicketsQuery(filter));
        await connection.CloseAsync();
        return new PagginationResult<TicketResponse[]>
        {
            Data = results.ToArray(),
            Page = filter.Page,
            Size = filter.Size,
            TotalCount = results.FirstOrDefault()?.TotalCount ?? 0, 
            TotalPages = results.FirstOrDefault()?.TotalCount ?? 0 / filter.Size
        };
    }
}