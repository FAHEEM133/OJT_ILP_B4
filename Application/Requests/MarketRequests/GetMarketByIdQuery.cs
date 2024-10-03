using Domain.Model;
using MediatR;

namespace Application.Requests.MarketRequests
{
    public class GetMarketByIdQuery : IRequest<Market>
    {
        public int MarketId { get; set; }

        public GetMarketByIdQuery(int marketId)
        {
            MarketId = marketId;
        }
    }
}
