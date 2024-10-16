using Domain.Model;
using MediatR;
using System.Collections.Generic;

namespace Application.Requests.MarketRequests
{
    public class GetAllMarketsQuery : IRequest<(List<Market> Markets, int TotalCount)>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
