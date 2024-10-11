using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Requests.SubGroupRequests;
using Application.DTOs; // Include the DTO namespace
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

        // GET: api/MarketSubGroup
        [HttpGet]
        public async Task<ActionResult<List<MarketSubGroupDTO>>> GetAllMarketSubGroups([FromQuery] string? marketCode)
        {
            try
            {
                var subGroups = await _mediator.Send(new GetAllMarketSubGroupsQuery(marketCode));
                return Ok(subGroups);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
