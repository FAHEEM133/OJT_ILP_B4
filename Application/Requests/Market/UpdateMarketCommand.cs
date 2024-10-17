using Application.DTOs;
using Domain.Enums;
using Domain.Enums.Domain.Enums;
using MediatR;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Command to update an existing market with new details, including name, code, long code, region, subregion, and subgroups.
/// </summary>
public class UpdateMarketCommand : IRequest<object>
{
    /// <summary>
    /// Gets or sets the unique identifier of the market to be updated.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the market.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the code of the market.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the long market code, a more detailed version of the market code.
    /// </summary>
    public string LongMarketCode { get; set; }

    /// <summary>
    /// Gets or sets the region to which the market belongs.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the subregion to which the market belongs.
    /// </summary>
    public SubRegion SubRegion { get; set; }

    /// <summary>
    /// Gets or sets the list of subgroups associated with the market.
    /// </summary>
    public List<MarketSubGroupDTO> MarketSubGroups { get; set; } = new List<MarketSubGroupDTO>();
}
