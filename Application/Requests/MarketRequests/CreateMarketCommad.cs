using Application.DTOs;
using Domain.Enums; // Import the enums
using Domain.Enums.Domain.Enums;
using MediatR;

namespace Application.Requests.MarketRequests
{
    public class CreateMarketCommand : IRequest<object>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string LongMarketCode { get; set; }

        public Region Region { get; set; }  // Add Region
        public SubRegion SubRegion { get; set; }  // Add SubRegion
        // Use MarketSubGroupDTO for SubGroups
        public List<MarketSubGroupDTO> MarketSubGroups { get; set; } = new List<MarketSubGroupDTO>();
    }
}
