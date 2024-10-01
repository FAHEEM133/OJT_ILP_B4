using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.SubGroupRequests
{
    public class GetMarketSubGroupByIdQueryHandler : IRequestHandler<GetMarketSubGroupByIdQuery, MarketSubGroupDTO>
    {
        private readonly AppDbContext _context;

        public GetMarketSubGroupByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MarketSubGroupDTO> Handle(GetMarketSubGroupByIdQuery request, CancellationToken cancellationToken)
        {
            var subGroup = await _context.MarketSubGroups
                .Include(subgroup => subgroup.Market)
                .FirstOrDefaultAsync(sg => sg.SubGroupId == request.SubGroupId, cancellationToken);

            if (subGroup == null) return null;

            // Map entity to DTO
            return new MarketSubGroupDTO
            {
                SubGroupId = subGroup.SubGroupId,
                SubGroupName = subGroup.SubGroupName,
                SubGroupCode = subGroup.SubGroupCode,
                MarketId = subGroup.MarketId,
                MarketCode = subGroup.Market.Code
            };
        }
    }
}
