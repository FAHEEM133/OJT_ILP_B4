using Application.Markets.Commands;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MarketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMarket(CreateMarketCommand command)
        {
            var result = await _mediator.Send(command);
            if (result == "Market created successfully")
            {
                return Ok(new { message = result });
            }

            return BadRequest(new { error = result });
        }

     /*   [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketById(int id)
        {
            var market = await _mediator.Send(new GetMarketByIdQuery(id));
            if (market == null)
            {
                return NotFound();
            }

            return Ok(market);
        }*/
    }
}
