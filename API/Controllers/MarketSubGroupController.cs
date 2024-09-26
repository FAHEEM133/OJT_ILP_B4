using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Requests.SubGroupRequests;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

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

        // GET: Get all MarketSubGroups
        [HttpGet]
        public async Task<ActionResult<List<MarketSubGroup>>> GetAllMarketSubGroups()
        {
            try
            {
                var subGroups = await _mediator.Send(new GetAllMarketSubGroupsQuery());
                return Ok(subGroups);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateMarketSubGroup([FromBody] CreateMarketSubGroupCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(CreateMarketSubGroup), new { id = result }, result);
            }
            catch (Exception ex)
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

            return NoContent(); // 204 No Content
        }
    }
}
