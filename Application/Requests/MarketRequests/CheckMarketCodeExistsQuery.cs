using MediatR;

namespace Application.Requests.MarketRequests
{
    public class CheckMarketCodeExistsQuery : IRequest<bool>
    {
        public string Code { get; set; }
    }
}
