using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetMarketDetailsByIdQuery : IRequest<MarketDetailsDto>
    {
        public int MarketId { get; set; }
    }
}
