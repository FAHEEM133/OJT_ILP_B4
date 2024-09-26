using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class CheckMarketNameExistsQueryHandler : IRequestHandler<CheckMarketNameExistsQuery, bool>
    {
        private readonly AppDbContext _appDbContext;

        /*
         * Constructor: CheckMarketNameExistsQueryHandler
         * Initializes the CheckMarketNameExistsQueryHandler with the application's database context.
         * 
         * Parameters:
         * - appDbContext: AppDbContext - The application's database context used to interact with the database.
         */
        public CheckMarketNameExistsQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /*
         * Method: Handle
         * Handles the CheckMarketNameExistsQuery to check if a market name already exists in the database.
         * 
         * Parameters:
         * - request: CheckMarketNameExistsQuery - The query object containing the market name to be checked.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<bool>: Asynchronously returns true if the market name exists, otherwise false.
         */
        public async Task<bool> Handle(CheckMarketNameExistsQuery request, CancellationToken cancellationToken)
        {
            // Step 1: Perform a case-insensitive comparison to check if the market name exists
            /*
             * LLD Steps:
             * 1.1. Use the _appDbContext to access the Markets DbSet.
             * 1.2. Use the AnyAsync method to check for any market entry where the Name matches the requested name in a case-insensitive manner.
             * 1.3. Convert both the database Name and the requested Name to lowercase using ToLower() to ensure case-insensitive comparison.
             * 1.4. Pass the cancellationToken to the AnyAsync method to allow for task cancellation if needed.
             * 1.5. Await the asynchronous call to AnyAsync and return true if a match is found, or false if not.
             */
            return await _appDbContext.Markets
                .AnyAsync(m => m.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        }
    }
}
