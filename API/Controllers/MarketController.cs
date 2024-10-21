using Application.Requests.MarketRequests;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MarketController : ControllerBase
{
    private readonly IMediator _mediator;


    public MarketController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost]
    public async Task<IActionResult> CreateMarket([FromBody] CreateMarketCommand command)
    {

        var marketId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMarketById), new { id = marketId }, marketId);
    }


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

    [HttpGet]
    public async Task<IActionResult> GetAllMarkets([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchText = null, [FromQuery] string? regions = null)
    {


        var query = new GetAllMarketsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchText = searchText,
            Regions = regions
        };


        var (markets, totalCount) = await _mediator.Send(query);

        return Ok(new
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Markets = markets
        });
    }


    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetMarketDetailsById(int id)
    {

        var marketDetails = await _mediator.Send(new GetMarketDetailsByIdQuery { Id = id });

        if (marketDetails == null)
            return NotFound($"Market with ID {id} not found");

        return Ok(marketDetails);
    }


    [HttpGet("code/{marketCode}/exists")]
    public async Task<IActionResult> CheckMarketCodeExists([FromRoute] string marketCode)
    {

        var exists = await _mediator.Send(new CheckMarketCodeExistsQuery { Code = marketCode });
        return Ok(exists);
    }



    [HttpGet("name/{marketName}/exists")]
    public async Task<IActionResult> CheckMarketNameExists([FromRoute] string marketName)
    {

        var exists = await _mediator.Send(new CheckMarketNameExistsQuery { Name = marketName });
        return Ok(exists);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMarket([FromRoute] int id, [FromBody] UpdateMarketCommand command)
    {


        try
        {
            command.Id = id;
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

        var result = await _mediator.Send(new DeleteMarketCommand { Id = id });

        return result ? NoContent() : NotFound();
    }


}
