using Application.DTOs;
using Domain.Enums; // Import the enums
using Domain.Enums.Domain.Enums;
using MediatR;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Command to create a new market with the provided details.
/// </summary>
public class CreateMarketCommand : IRequest<object>
{
    /// <summary>
    /// Gets or sets the name of the market to be created.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the market code.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the long version of the market code.
    /// </summary>
    public string LongMarketCode { get; set; }

    /// <summary>
    /// Gets or sets the region where the market is located.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the subregion where the market is located.
    /// </summary>
    public SubRegion SubRegion { get; set; }

    /// <summary>
    /// Gets or sets the list of market subgroups associated with the market.
    /// </summary>
    public List<MarketSubGroupDTO> MarketSubGroups { get; set; } = new List<MarketSubGroupDTO>();
}
