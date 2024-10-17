using Application.DTOs;
using MediatR;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Represents a query to retrieve the details of a market by its unique identifier.
/// </summary>
public class GetMarketDetailsByIdQuery : IRequest<MarketDetailsDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the market to retrieve its details.
    /// </summary>
    public int MarketId { get; set; }
}
