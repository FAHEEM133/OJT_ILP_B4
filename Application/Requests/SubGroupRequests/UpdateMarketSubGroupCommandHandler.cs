using MediatR;
using Domain.Model;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{

    /**
     * @class UpdateMarketSubGroupCommandHandler
     * Handles the update operation for an existing MarketSubGroup entity.
     */
    public class UpdateMarketSubGroupCommandHandler : IRequestHandler<UpdateMarketSubGroupCommand, int>
    {
        private readonly AppDbContext _context;

        /**
         * Constructor
         * @param context AppDbContext - Injected database context for interacting with MarketSubGroup entities.
         */
        public UpdateMarketSubGroupCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the update command for an existing MarketSubGroup.
         * @param request UpdateMarketSubGroupCommand - Contains the updated data for the MarketSubGroup.
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Retrieves the existing MarketSubGroup from the database, updates its properties, and saves the changes.
         * 
         * @return int - Returns the SubGroupId of the updated MarketSubGroup.
         * @throws KeyNotFoundException - If the MarketSubGroup is not found.
         */
        public async Task<int> Handle(UpdateMarketSubGroupCommand request, CancellationToken cancellationToken)
        {
            // Find the existing MarketSubGroup
            var existingSubGroup = await _context.MarketSubGroups
                .FirstOrDefaultAsync(sg => sg.SubGroupId == request.SubGroupId, cancellationToken);

            if (existingSubGroup == null)
            {
                // Optionally throw exception or return a specific error code if the entity is not found.
                throw new KeyNotFoundException($"MarketSubGroup with Id {request.SubGroupId} not found.");
            }

            // Update the properties
            existingSubGroup.SubGroupName = request.SubGroupName;
            existingSubGroup.SubGroupCode = request.SubGroupCode;
            existingSubGroup.MarketId = request.MarketId;
            existingSubGroup.UpdatedAt = DateTime.UtcNow;  // Update the timestamp

            // Save changes to the database
            _context.MarketSubGroups.Update(existingSubGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return existingSubGroup.SubGroupId;  // Return the updated SubGroupId
        }
    }
}
