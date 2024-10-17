using Application.Validations;
using MediatR;


namespace Application.Requests.RegionRequests;

/// <summary>
/// Handles the query to retrieve subregions for a specific region.
/// Returns subregions as key-value pairs where the key is the subregion ID and the value is the subregion name.
/// </summary>
public class GetSubRegionsByRegionQueryHandler : IRequestHandler<GetSubRegionsByRegionQuery, List<KeyValuePair<int, string>>>
{

    /// <summary>
    /// Handles the query by fetching subregions for a specified region using the <see cref="RegionSubRegionValidation"/> helper.
    /// Converts the subregions into key-value pairs.
    /// </summary>
    /// <param name="request">The request containing the region for which subregions are to be retrieved.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A list of key-value pairs representing subregion IDs and names for the specified region.</returns>
    public Task<List<KeyValuePair<int, string>>> Handle(GetSubRegionsByRegionQuery request, CancellationToken cancellationToken)
    {

        var subRegions = RegionSubRegionValidation.GetSubRegionsForRegion(request.Region);


        var subRegionList = subRegions
                            .Select(subRegion => new KeyValuePair<int, string>((int)subRegion, subRegion.ToString()))
                            .ToList();

        return Task.FromResult(subRegionList);
    }
}
