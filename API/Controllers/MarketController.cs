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
         * Method: Create
         * Handles the HTTP POST request to create a new market entry.
         * 
         * Parameters:
         * - command: CreateMarketCommand - The command object containing details for creating a new market.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns a CreatedAtAction result containing the ID of the newly created market.
         */
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMarketCommand command)
        {
            /*
             * LLD Steps:
             * 1. Send the CreateMarketCommand to the mediator for processing.
             * 2. Await the result which provides the ID of the newly created market.
             * 3. Use CreatedAtAction to return a response with a link to the GetMarketById method, passing the created market ID.
             */
            var marketId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMarketById), new { id = marketId }, marketId);
        }

        /*
         * Method: GetMarketById
         * Handles the HTTP GET request to retrieve a market entry by its ID.
         * 
         * Parameters:
         * - id: int - The unique identifier of the market to retrieve.
         * 
         * Returns:
         * - Task<IActionResult>: Asynchronously returns the market entry if found; otherwise, returns NotFound.
         */
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketById(int id)
        {
            /*
             * LLD Steps:
             * 1. Create a GetMarketByIdQuery with the provided ID.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which should be the market entity matching the provided ID.
             * 4. Return the market entity in an Ok response if found, or NotFound if no entity exists with that ID.
             */
            var market = await _mediator.Send(new GetMarketDetailsByIdQuery { MarketId = id });
            return market != null ? Ok(market) : NotFound();
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

