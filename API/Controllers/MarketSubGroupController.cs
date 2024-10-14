using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Requests.SubGroupRequests;
using Application.DTOs; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketSubGroupController : ControllerBase
    {
        private readonly IMediator _mediator;
       /*
        * Constructor: MarketSubGroupController
        * Initializes the MarketSubGroupController with the IMediator instance for handling requests.
        * 
        * Parameters:
        * - mediator: IMediator - The MediatR mediator used for sending commands and queries.
        */
        public MarketSubGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*
         * Method: GetAllMarketSubGroups
         * Handles the HTTP GET request to retrieve all market subgroups, with an optional marketId filter.
         * 
         * Parameters:
         * - marketId: int? (optional) - The optional identifier for filtering subgroups based on the associated market.
         * 
         * Returns:
         * - Task<ActionResult<List<MarketSubGroupDTO>>>: Asynchronously returns a list of market subgroups. If no marketId is provided, returns all subgroups.
         */

        [HttpGet]
        public async Task<ActionResult<List<MarketSubGroupDTO>>> GetAllMarketSubGroups([FromQuery] int? marketId)
        {
            /*
             * LLD Steps:
             * 1. Create a GetAllMarketSubGroupsQuery object with the optional marketId filter.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which will be a list of MarketSubGroupDTOs.
             * 4. If successful, return the list of subgroups in an Ok response.
             * 5. If an exception occurs, catch it and return a BadRequest response with the exception message.
             */
            try
            {
                var subGroups = await _mediator.Send(new GetAllMarketSubGroupsQuery(marketId));
                return Ok(subGroups);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
