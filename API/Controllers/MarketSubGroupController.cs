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

        public MarketSubGroupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<MarketSubGroupDTO>>> GetAllMarketSubGroups([FromQuery] int? marketId)
        {

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
