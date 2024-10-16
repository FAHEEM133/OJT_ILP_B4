using Domain.Enums;
using MediatR;

namespace Application.Requests.RegionRequests;

/// <summary>
/// Query to retrieve subregions for a specified region.
/// Returns the subregions as key-value pairs where the key is the subregion ID and the value is the subregion name.
/// </summary>
public class GetSubRegionsByRegionQuery : IRequest<List<KeyValuePair<int, string>>>
{
    /// <summary>
    /// Gets or sets the region for which subregions are to be retrieved.
    /// </summary>
    public Region Region { get; set; }
}
