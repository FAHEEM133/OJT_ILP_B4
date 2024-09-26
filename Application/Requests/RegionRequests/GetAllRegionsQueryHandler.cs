using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<KeyValuePair<int, string>>>
    {
        /*
         * Method: Handle
         * Retrieves all available regions with their ID and name.
         * 
         * Parameters:
         * - request: GetAllRegionsQuery - The query object for fetching all regions.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<List<KeyValuePair<int, string>>>: Asynchronously returns a list of regions with their ID and name.
         */
        public Task<List<KeyValuePair<int, string>>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Use `Enum.GetValues` to retrieve all values from the Region enum.
             * 2. Cast the retrieved values to the `Region` type using `Cast<Region>()`.
             * 3. Create a list of `KeyValuePair<int, string>` containing both ID and Name.
             * 4. Return the list wrapped in a `Task` using `Task.FromResult`.
             */
            var regions = Enum.GetValues(typeof(Region))
                              .Cast<Region>()
                              .Select(region => new KeyValuePair<int, string>((int)region, region.ToString()))
                              .ToList();

            return Task.FromResult(regions);
        }
    }
}
