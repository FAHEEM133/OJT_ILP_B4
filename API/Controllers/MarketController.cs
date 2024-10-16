using Application.Requests.MarketRequests;
using FluentValidation;
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
         * Handles the HTTP GET request to retrieve a market entry by its ID and to retireve the Details.
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
            var market = await _mediator.Send(new GetMarketByIdQuery(id));
            if (market == null)
            {
                return NotFound("Market not found.");
            }

            return Ok(market);
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
        * Method: Update
        * Handles the HTTP PUT request to update an existing market entry.
        * 
        * Parameters:
        * - id: int - The ID of the market to update (from the route).
        * - command: UpdateMarketCommand - The command object containing updated market details (from the request body).
        * 
        * Returns:
        * - Task<IActionResult>: Asynchronously returns a response indicating the result of the update operation.
        */

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMarketCommand command)
        {
            /*
             LLD Steps:
             * 1. Validate that the market ID provided in the URL route matches the ID in the command object (request body).
             *    - If the IDs don't match, return a BadRequest response with an appropriate error message.
             * 2. Send the `UpdateMarketCommand` to the mediator to handle the update logic.
             *    - The `UpdateMarketCommand` contains the updated market data and is processed by the respective handler.
             * 3. Await the result from the mediator, which provides the ID of the updated market.
             * 4. If the update is successful, return an Ok response with a success message and the updated market ID.
             * 5. If a validation exception occurs (e.g., validation rules defined for the command are violated), catch the exception.
             *    - Return a BadRequest response with the exception message detailing the validation failure.
             
             */
            if (id != command.Id)
            {
                return BadRequest(new { message = "ID in the URL does not match the ID in the request body." });
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarketById(int id)
        {
            /*
             * LLD Steps:
            * 1. Create a DeleteMarketByIdCommand with the provided ID.
             * 2. Send the command to the mediator for processing.
             * 3. Await the result, which should indicate whether the deletion was successful.
             * 4. Return NoContent if the deletion was successful, or NotFound if no entity exists with that ID.
            */
            var result = await _mediator.Send(new DeleteMarketCommand { Id = id });

            return result ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMarket([FromQuery] string searchText)
        {
            /*
             * LLD Steps:
             * 1. Create a new SearchMarketQuery object and pass the searchText.
             * 2. Send the query to the mediator for processing.
             * 3. Await the result, which will be a list of markets matching the search text.
             * 4. Return the list of market entities in an Ok response, or return NotFound if no matches are found.
             */
            var markets = await _mediator.Send(new SearchMarketQuery
            {
                SearchText = searchText
            });

            if (markets == null || markets.Count == 0)
            {
                return NotFound("No markets found with the given search criteria.");
            }

            return Ok(markets);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterMarkets([FromQuery] FilterMarketsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}

