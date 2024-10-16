using Domain.Model;
using MediatR;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Query to fetch all markets with pagination.
/// </summary>
public class GetAllMarketsQuery : IRequest<(List<Market> Markets, int TotalCount)>
{
    /// <summary>
    /// Gets or sets the page number for the markets to be fetched. Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of markets to fetch per page. Defaults to 10.
    /// </summary>
    public int PageSize { get; set; } = 10;
}
