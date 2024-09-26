﻿using Application.Requests.MarketRequests;
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
            var market = await _mediator.Send(new GetMarketByIdQuery { Id = id });
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
    var result = await _mediator.Send(new DeleteMarketByIdCommand { Id = id });
    
    return result ? NoContent() : NotFound();
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

    }
}
