using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Application.Requests.SubGroupRequests
{
    public class UpdateMarketSubGroupCommandHandler : IRequestHandler<UpdateMarketSubGroupCommand, MarketSubGroupDTO>
    {
        private readonly AppDbContext _context;

        public UpdateMarketSubGroupCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MarketSubGroupDTO> Handle(UpdateMarketSubGroupCommand request, CancellationToken cancellationToken)
        {
            // Fetch the existing MarketSubGroup entity
            var subGroup = await _context.MarketSubGroups
                .Include(sg => sg.Market) // Include the related Market entity
                .FirstOrDefaultAsync(sg => sg.SubGroupId == request.SubGroupId, cancellationToken);

            // Check if the entity was found
            if (subGroup == null) return null;

            // Update the MarketSubGroup entity with new data from the request
            subGroup.SubGroupName = request.SubGroupName;
            subGroup.SubGroupCode = request.SubGroupCode;
            subGroup.MarketId = request.MarketId;
            subGroup.UpdatedAt = DateTime.UtcNow;

            // Save the changes in the database
            await _context.SaveChangesAsync(cancellationToken);

            // Return the updated entity as a DTO
            return new MarketSubGroupDTO
            {
                SubGroupId = subGroup.SubGroupId,
                SubGroupName = subGroup.SubGroupName,
                SubGroupCode = subGroup.SubGroupCode,
                MarketId = subGroup.MarketId,
                MarketCode = subGroup.Market.Code // Map MarketCode from the related Market entity
            };
        }
    }
}
