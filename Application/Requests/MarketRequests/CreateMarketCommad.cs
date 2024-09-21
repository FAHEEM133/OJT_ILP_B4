using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class CreateMarketCommand : IRequest<int>
    {
        public string MarketName { get; set; }
        public string MarketCode { get; set; }
        public string LongMarketCode { get; set; }
    }
}
