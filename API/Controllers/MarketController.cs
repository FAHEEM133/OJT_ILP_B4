using Application.Requests.MarketRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly IMediator _mediator;

        /*
         * Constructor: MarketController
         * Initializes the MarketController with the IMediator instance for handling requests.
         * 
         * Parameters:
         * - mediator: IMediator - The MediatR mediator used for sending commands and queries.
         */
        public MarketController(IMediator mediator)
        {
            _mediator = mediator;
        }

   
        /*
         * Method: GetAllMarkets
         * Handles the HTTP GET request to retrieve all market entries.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns a list of all market entries.
         */
        [HttpGet]
        public async Task<IActionResult> GetAllMarkets()
        {
            /*
             * LLD Steps:
             * 1. Create a new GetAllMarketsQuery.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which should be a list of all market entities.
             * 4. Return the list of market entities in an Ok response.
             */
            var markets = await _mediator.Send(new GetAllMarketsQuery());
            return Ok(markets);
        }

        /*
         * Method: CheckMarketCodeExists
         * Handles the HTTP GET request to check if a market code already exists in the database.
         * 
         * Parameters:
         * - marketCode: string - The market code to check for existence.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns true if the market code exists; otherwise, returns false.
         */
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckMarketCodeExists([FromQuery] string marketCode)
        {
            /*
             * LLD Steps:
             * 1. Create a CheckMarketCodeExistsQuery object with the provided marketCode.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which will be a boolean value indicating whether the market code exists.
             * 4. Return the result in an Ok response, which will be true if the market code exists, and false otherwise.
             */
            var exists = await _mediator.Send(new CheckMarketCodeExistsQuery { Code = marketCode });
            return Ok(exists);
        }

        /*
         * Method: CheckMarketNameExists
         * Handles the HTTP GET request to check if a market name already exists in the database.
         * 
         * Parameters:
         * - marketName: string - The market name to check for existence.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns true if the market name exists; otherwise, returns false.
         */
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckMarketNameExists([FromQuery] string marketName)
        {
            /*
             * LLD Steps:
             * 1. Create a CheckMarketNameExistsQuery object with the provided marketName.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which will be a boolean value indicating whether the market name exists.
             * 4. Return the result in an Ok response, which will be true if the market name exists, and false otherwise.
             */
            var exists = await _mediator.Send(new CheckMarketNameExistsQuery { Name = marketName });
            return Ok(exists);
        }

        /*
          * Method: GetMarketDetailsById
          * Handles the HTTP GET request to retrieve the details of a specific market by its ID.
          * 
          * Parameters:
          * - id: int - The ID of the market whose details are to be retrieved.
          * 
          * Returns:
          * - Task<IActionResult>: Asynchronously returns the details of the market if found, or a NotFound response if no such market exists.
  */
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetMarketDetailsById(int id)
        {
            /*
             * LLD Steps:
             * 1. Create a GetMarketDetailsByIdQuery object with the provided marketId.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which will be the details of the market with the given ID.
             * 4. If the market is found, return the market details in an Ok response.
             * 5. If no market is found, return a NotFound response with a message indicating the missing market ID.
             */
            var marketDetails = await _mediator.Send(new GetMarketDetailsByIdQuery { MarketId = id });

            if (marketDetails == null)
                return NotFound($"Market with ID {id} not found");

            return Ok(marketDetails);
        }

    }
}

