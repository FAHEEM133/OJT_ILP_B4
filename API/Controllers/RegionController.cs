using Application.Requests.RegionRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Enums;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IMediator _mediator;

        /*
         * Constructor: RegionController
         * Initializes the RegionController with the IMediator instance for handling requests.
         * 
         * Parameters:
         * - mediator: IMediator - The MediatR mediator used for sending commands and queries.
         */
        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*
         * Method: GetAllRegions
         * Handles the HTTP GET request to retrieve all available regions.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns a list of all regions.
         */
        [HttpGet("regions")]
        public async Task<IActionResult> GetAllRegions()
        {
            /*
             * LLD Steps:
             * 1. Create and send a GetAllRegionsQuery to the mediator.
             * 2. Await the response, which should be a list of all regions from the enum.
             * 3. Return the list of regions in an Ok response.
             */
            var regions = await _mediator.Send(new GetAllRegionsQuery());
            return Ok(regions);
        }

        /*
         * Method: GetSubRegionsByRegion
         * Handles the HTTP GET request to retrieve subregions based on a specified region.
         * 
         * Parameters:
         * - region: Region - The main region for which subregions need to be retrieved.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns a list of subregions for the specified region.
         */
        [HttpGet("{region}/subregions")]
        public async Task<IActionResult> GetSubRegionsByRegion(Region region)
        {
            /*
             * LLD Steps:
             * 1. Create a GetSubRegionsByRegionQuery object, passing the provided region.
             * 2. Send the query to the mediator for processing.
             * 3. Await the response, which should be a list of subregions associated with the specified region.
             * 4. Return the list of subregions in an Ok response.
             */
            var subRegions = await _mediator.Send(new GetSubRegionsByRegionQuery { Region = region });
            return Ok(subRegions);
        }
    }
}
