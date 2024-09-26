using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{

    /**
     * @class DeleteMarketSubGroupCommandHandler
     * Handles the deletion of a MarketSubGroup from the database. Validates the existence of the SubGroup and removes it if found.
     */
    public class DeleteMarketSubGroupCommandHandler : IRequestHandler<DeleteMarketSubGroupCommand, bool>
    {
        private readonly AppDbContext _context;

        /**
         * Constructor
         * @param context AppDbContext - Injected database context for interacting with MarketSubGroup entities.
         */
        public DeleteMarketSubGroupCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the command to delete a MarketSubGroup.
         * @param request DeleteMarketSubGroupCommand - Contains the SubGroupId to be deleted.
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Searches for the MarketSubGroup by ID and removes it if found.
         * 
         * @return bool - Returns true if the MarketSubGroup is successfully deleted, or false if it doesn't exist.
         */
        public async Task<bool> Handle(DeleteMarketSubGroupCommand request, CancellationToken cancellationToken)
        {
            var marketSubGroup = await _context.MarketSubGroups.FindAsync(request.SubGroupId);

            if (marketSubGroup == null)
            {
                return false;
            }
            _context.MarketSubGroups.Remove(marketSubGroup);

            
            await _context.SaveChangesAsync(cancellationToken);

            return true; 
        }
    }
}
