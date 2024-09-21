using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetMarketByIdQueryHandler : IRequestHandler<GetMarketByIdQuery, Market>
    {
        private readonly AppDbContext _context;

        public GetMarketByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Market> Handle(GetMarketByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Markets.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
