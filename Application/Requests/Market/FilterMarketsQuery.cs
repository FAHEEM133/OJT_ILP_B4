using Application.DTOs;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class FilterMarketsQuery : IRequest<List<MarketDetailsDto>>
    {
        public string Regions { get; set; }

    }
}
