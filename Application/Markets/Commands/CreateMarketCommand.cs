using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Markets.Commands
{
    public class CreateMarketCommand : IRequest<string>
    {
        public string MarketName { get; set; }
        public string MarketCode { get; set; }
        public string LongMarketCode { get; set; }
     
    }
}
