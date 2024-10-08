using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, (List<Market> Markets, int TotalCount)>
    {
        private readonly AppDbContext _context;

        public GetAllMarketsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Market> Markets, int TotalCount)> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
        {
            // Get total count of all market records
            var totalCount = await _context.Markets.CountAsync(cancellationToken);

            // Fetch paginated data
            var markets = await _context.Markets
                                        .Include(m => m.MarketSubGroups)
                                        .Skip((request.PageNumber - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync(cancellationToken);

            // Return markets and total count as a tuple
            return (markets, totalCount);
        }
    }
}
