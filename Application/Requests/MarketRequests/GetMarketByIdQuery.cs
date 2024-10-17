using Domain.Model;
using MediatR;

namespace Application.Requests.MarketRequests;
/// <summary>
/// Represents a query to retrieve a market by its unique identifier.
/// </summary>
public class GetMarketByIdQuery : IRequest<Market>
{
    /// <summary>
    /// Gets or sets the unique identifier of the market to be retrieved.
    /// </summary>
    public int MarketId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMarketByIdQuery"/> class.
    /// </summary>
    /// <param name="marketId">The unique identifier of the market to be retrieved.</param>
    public GetMarketByIdQuery(int marketId)
    {
        MarketId = marketId;
    }
}
