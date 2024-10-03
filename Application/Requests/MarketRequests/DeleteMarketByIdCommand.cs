using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.MarketRequests
{
    public class DeleteMarketCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
    public class DeleteMarketCommandHandler : IRequestHandler<DeleteMarketCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
  * Method: Handle
  * Responsibility: Deletes a market by ID, but only if there are no associated subgroups.
  * 
  * Steps:
  * 1. Retrieve the market by ID, including any subgroups.
  * 2. If the market is not found, return false (no deletion).
  * 3. Check if the market has any subgroups.
  * 4. If the market has subgroups, return false (cannot delete).
  * 5. If no subgroups exist, proceed to delete the market.
  * 6. Save the changes to the database and return true (deletion successful).
  */
        public async Task<bool> Handle(DeleteMarketCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Retrieve the market by ID, including its subgroups
            var market = await _context.Markets
                                       .Include(m => m.MarketSubGroups) // Including subgroups related to the market
                                       .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            // Step 2: If the market is not found, return false
            if (market == null)
            {
                return false; // Market with the specified ID not found
            }

            // Step 3: Check if the market has any associated subgroups
            if (market.MarketSubGroups != null && market.MarketSubGroups.Count > 0)
            {
                return false; // Market has subgroups, so it can't be deleted
            }

            // Step 5: Proceed to delete the market if no subgroups are found
            _context.Markets.Remove(market); // Removing the market from the DbSet

            // Step 6: Save changes to the database
            await _context.SaveChangesAsync(cancellationToken); // Save changes asynchronously

            return true; // Deletion successful
        }

    }
}
