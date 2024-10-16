using MediatR;


namespace Application.Requests.RegionRequests;

/// <summary>
/// Query to retrieve all regions in the form of key-value pairs where
/// the key is the region ID and the value is the region name.
/// </summary>
public class GetAllRegionsQuery : IRequest<List<KeyValuePair<int, string>>>
{
}
