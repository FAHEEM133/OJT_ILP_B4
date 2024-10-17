using Application.DTOs;
using MediatR;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Represents a query to search for markets by a text input.
/// </summary>
public class SearchMarketQuery : IRequest<List<MarketDetailsDto>>
{
    /// <summary>
    /// Gets or sets the search text used to find markets by name, code, or long market code.
    /// </summary>
    public string SearchText { get; set; }
}
