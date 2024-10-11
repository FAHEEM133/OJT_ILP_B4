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
    public class GetAllMarketSubGroupsQuery : IRequest<List<MarketSubGroupDTO>>
    {
        public string? MarketCode { get; set; }

        public GetAllMarketSubGroupsQuery(string? marketCode = null)
        {
            MarketCode = marketCode;
        }
    }
}
