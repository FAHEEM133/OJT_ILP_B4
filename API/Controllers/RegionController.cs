using Application.Requests.RegionRequests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Enums;
using System.Collections.Generic;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Endpoint to get all regions
        [HttpGet("all-regions")]
        public async Task<IActionResult> GetAllRegions()
        {
            var regions = await _mediator.Send(new GetAllRegionsQuery());
            return Ok(regions);
        }

        // Endpoint to get subregions by region
        [HttpGet("{region}/subregions")]
        public async Task<IActionResult> GetSubRegionsByRegion(Region region)
        {
            var subRegions = await _mediator.Send(new GetSubRegionsByRegionQuery { Region = region });
            return Ok(subRegions);
        }
    }
}
