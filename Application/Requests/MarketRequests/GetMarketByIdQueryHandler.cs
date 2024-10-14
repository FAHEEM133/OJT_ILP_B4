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
            
            var market = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);

            
            if (market == null)
            {
                return null;
            }

            return market;
        }
    }
}
