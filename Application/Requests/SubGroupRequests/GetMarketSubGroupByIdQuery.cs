using Application.DTOs;
using Domain.Model;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class GetMarketSubGroupByIdQuery : IRequest<MarketSubGroupDTO>
    {
        public int SubGroupId { get; set; }

        public GetMarketSubGroupByIdQuery(int subGroupId)
        {
            SubGroupId = subGroupId;
        }
    }
}
