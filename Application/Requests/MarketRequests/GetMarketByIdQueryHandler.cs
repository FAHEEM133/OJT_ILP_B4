using Application.Requests.MarketRequests;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handlers.MarketHandlers
{
    public class GetMarketByIdHandler : IRequestHandler<GetMarketByIdQuery, Market>
    {
        private readonly AppDbContext _context;

        public GetMarketByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Market> Handle(GetMarketByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch the market details by MarketId
            var market = await _context.Markets
                .Include(m => m.MarketSubGroups) // Optionally include related data if needed
                .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);

            // Return the market entity or handle the case where it's not found
            if (market == null)
            {
                return null; // Or throw an exception if needed, e.g., throw new NotFoundException("Market not found");
            }

            return market;
        }
    }
}
