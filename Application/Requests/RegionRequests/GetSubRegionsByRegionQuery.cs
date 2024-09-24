using Domain.Enums;
using Domain.Enums.Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace Application.Requests.RegionRequests
{
    /*
     * Class: GetSubRegionsByRegionQuery
     * Represents a query to retrieve subregions based on a selected region.
     * Implements the IRequest interface from MediatR to handle the request-response mechanism.
     * 
     * Properties:
     * - Region: Region - The selected region for which subregions need to be retrieved.
     * 
     * Returns:
     * - IRequest<List<SubRegion>>: The request expects a response containing a list of SubRegion objects.
     */
    public class GetSubRegionsByRegionQuery : IRequest<List<SubRegion>>
    {
        /*
         * Property: Region
         * Represents the selected region from which the subregions will be retrieved.
         */
        public Region Region { get; set; }
    }
}
