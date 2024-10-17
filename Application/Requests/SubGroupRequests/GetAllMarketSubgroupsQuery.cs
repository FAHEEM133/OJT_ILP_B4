using Application.DTOs;
using MediatR;


namespace Application.Requests.SubGroupRequests;

/// <summary>
/// Query to retrieve all market subgroups.
/// Optionally, if a MarketId is provided, retrieves subgroups for the specified market.
/// </summary>
public class GetAllMarketSubGroupsQuery : IRequest<List<MarketSubGroupDTO>>
{
    /// <summary>
    /// Gets or sets the optional MarketId.
    /// If provided, subgroups will be filtered by this market ID.
    /// If null, all market subgroups will be retrieved.
    /// </summary>
    public int? MarketId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllMarketSubGroupsQuery"/> class.
    /// </summary>
    /// <param name="marketId">The optional market ID to filter subgroups. Default is null.</param>
    public GetAllMarketSubGroupsQuery(int? marketId = null)
    {
        MarketId = marketId;
    }
}
