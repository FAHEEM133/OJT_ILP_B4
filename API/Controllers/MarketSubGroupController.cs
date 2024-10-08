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

        // GET: api/MarketSubGroup/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMarketSubGroupById(int id)
        {
            var subGroup = await _mediator.Send(new GetMarketSubGroupByIdQuery(id));

            if (subGroup == null)
            {
                return NotFound(new { Message = "MarketSubGroup not found" });
            }

            return Ok(subGroup);
        }

        // PUT: api/MarketSubGroup/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMarketSubGroup(int id, [FromBody] UpdateMarketSubGroupCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != command.SubGroupId)
            {
                return BadRequest(new { Message = "SubGroup ID mismatch" });
            }

            try
            {
                var result = await _mediator.Send(command);
                return Ok(new { Message = "MarketSubGroup updated successfully", Id = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/MarketSubGroup/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarketSubGroup(int id)
        {
            var result = await _mediator.Send(new DeleteMarketSubGroupCommand(id));

            if (!result)
            {
                return NotFound(new { Message = "MarketSubGroup not found" });
            }

            return NoContent();
        }
    }
}
