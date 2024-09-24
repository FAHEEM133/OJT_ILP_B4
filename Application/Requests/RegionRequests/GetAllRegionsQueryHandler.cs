using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<Region>>
    {
        /*
         * Method: Handle
         * Retrieves all available regions from the Region enum.
         * 
         * Parameters:
         * - request: GetAllRegionsQuery - The query object for fetching all regions.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<List<Region>>: Asynchronously returns a list of all Region enum values.
         */
        public Task<List<Region>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Use the `Enum.GetValues` method to retrieve all values from the Region enum type.
             * 2. Cast the retrieved enum values to the `Region` type using `Cast<Region>()`.
             * 3. Convert the casted values to a list using `ToList()`.
             * 4. Return the list of regions wrapped in a `Task` using `Task.FromResult`.
             */
            var regions = Enum.GetValues(typeof(Region)).Cast<Region>().ToList();
            return Task.FromResult(regions);
        }
    }
}
