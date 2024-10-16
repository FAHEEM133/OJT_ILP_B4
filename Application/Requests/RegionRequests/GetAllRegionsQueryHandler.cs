using Domain.Enums;
using MediatR;


namespace Application.Requests.RegionRequests;

/// <summary>
/// Handles the query to retrieve all regions, returning them as key-value pairs where the key is the region ID and the value is the region name.
/// </summary>
public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<KeyValuePair<int, string>>>
{

    /// <summary>
    /// Handles the query to retrieve all regions by converting the <see cref="Region"/> enum values into key-value pairs.
    /// </summary>
    /// <param name="request">The request to retrieve all regions.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A list of key-value pairs representing the region ID and name.</returns>
    public Task<List<KeyValuePair<int, string>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
    {

        var regions = Enum.GetValues(typeof(Region))
                          .Cast<Region>()
                          .Select(region => new KeyValuePair<int, string>((int)region, region.ToString()))
                          .ToList();

        return Task.FromResult(regions);
    }
}
