using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    /**
     * @class CreateMarketSubGroupCommandHandler
     * Handles the creation of a new MarketSubGroup by validating the existence of the Market, 
     * ensuring the uniqueness of the SubGroup, and saving it to the database.
     */
    public class CreateMarketSubGroupCommandHandler : IRequestHandler<CreateMarketSubGroupCommand, int>
    {
        private readonly AppDbContext _context;
       /**
        * Constructor
        * @param context AppDbContext - Injected database context for interacting with Market and MarketSubGroup entities.
        */

        public CreateMarketSubGroupCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the command to create a MarketSubGroup.
         * @param request CreateMarketSubGroupCommand - Contains the SubGroup data.
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Validates Market existence, ensures SubGroup name/code uniqueness, 
         * creates the SubGroup, and saves it to the database.
         * 
         * @return int - The ID of the newly created MarketSubGroup.
         * @throws Exception - If Market is not found or if SubGroup name/code is not unique.
         */
        public async Task<int> Handle(CreateMarketSubGroupCommand request, CancellationToken cancellationToken)
        {
            // Validate if the Market exists
            var market = await _context.Markets.FindAsync(request.MarketId);
            if (market == null)
            {
                throw new Exception("Market not found");
            }

            var exists = await _context.MarketSubGroups.AnyAsync(
                sg => sg.MarketId == request.MarketId &&
                      (sg.SubGroupName == request.SubGroupName || sg.SubGroupCode == request.SubGroupCode),
                cancellationToken
            );

            if (exists)
            {
                throw new Exception("A subgroup with the same name or code already exists within this market.");
            }

            
            var marketSubGroup = new MarketSubGroup
            {
                SubGroupName = request.SubGroupName,
                SubGroupCode = request.SubGroupCode,
                MarketId = request.MarketId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MarketSubGroups.Add(marketSubGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return marketSubGroup.SubGroupId;
        }
    }
}
