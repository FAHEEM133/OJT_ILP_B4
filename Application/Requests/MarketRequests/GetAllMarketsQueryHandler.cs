using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, List<Market>>
    {
        private readonly AppDbContext _context;

        public GetAllMarketsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Market>> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Markets.ToListAsync(cancellationToken);
        }
    }
}
