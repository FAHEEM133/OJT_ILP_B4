using Application.Validations;
using Domain.Enums;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetSubRegionsByRegionQueryHandler : IRequestHandler<GetSubRegionsByRegionQuery, List<KeyValuePair<int, string>>>
    {
        /*
         * Method: Handle
         * Retrieves all subregions for a given region.
         * 
         * Parameters:
         * - request: GetSubRegionsByRegionQuery - The query containing the selected region.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<List<KeyValuePair<int, string>>>: Asynchronously returns a list of subregions with their IDs and names.
         */
        public Task<List<KeyValuePair<int, string>>> Handle(GetSubRegionsByRegionQuery request, CancellationToken cancellationToken)
        {
            // Get all subregions based on the region using the validation logic
            var subRegions = RegionSubRegionValidation.GetSubRegionsForRegion(request.Region);

            // Convert subRegions enum to a list of key-value pairs (ID and Name)
            var subRegionList = subRegions
                                .Select(subRegion => new KeyValuePair<int, string>((int)subRegion, subRegion.ToString()))
                                .ToList();

            return Task.FromResult(subRegionList);
        }
    }
}
