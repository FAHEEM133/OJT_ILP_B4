using Domain.Enums; // Import the enums
using Domain.Enums.Domain.Enums;
using MediatR;

namespace Application.Requests.MarketRequests
{
    public class CreateMarketCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string LongMarketCode { get; set; }

        public Region Region { get; set; }  // Add Region
        public SubRegion SubRegion { get; set; }  // Add SubRegion
    }
}
