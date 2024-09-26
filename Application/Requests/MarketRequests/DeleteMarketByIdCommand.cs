using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Requests.MarketRequests
{
    public class DeleteMarketByIdCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
