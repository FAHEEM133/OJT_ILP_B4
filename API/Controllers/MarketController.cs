using Application.Requests.MarketRequests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
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
        public async Task<IActionResult> Create([FromBody] CreateMarketCommand command)
        {
            var marketId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = marketId }, marketId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var market = await _mediator.Send(new GetMarketByIdQuery { Id = id });
            return market != null ? Ok(market) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var markets = await _mediator.Send(new GetAllMarketsQuery());
            return Ok(markets);
        }

      

       
    }
}
