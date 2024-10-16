using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class CheckMarketCodeExistsQueryHandler : IRequestHandler<CheckMarketCodeExistsQuery, bool>
    {
        private readonly AppDbContext _appDbContext;

        /*
         * Constructor: CheckMarketCodeExistsQueryHandler
         * Initializes the CheckMarketCodeExistsQueryHandler with the application's database context.
         * 
         * Parameters:
         * - appDbContext: AppDbContext - The application's database context used to interact with the database.
         */
        public CheckMarketCodeExistsQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /*
         * Method: Handle
         * Handles the CheckMarketCodeExistsQuery to check if a market code already exists in the database.
         * 
         * Parameters:
         * - request: CheckMarketCodeExistsQuery - The query object containing the market code to be checked.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<bool>: Asynchronously returns true if the market code exists, otherwise false.
         */
        public async Task<bool> Handle(CheckMarketCodeExistsQuery request, CancellationToken cancellationToken)
        {
            // Step 1: Perform a case-insensitive comparison to check if the market code exists
            /*
             * LLD Steps:
             * 1.1. Use the _appDbContext to access the Markets DbSet.
             * 1.2. Use the AnyAsync method to check for any market entry where the Code matches the requested code in a case-insensitive manner.
             * 1.3. Convert both the database Code and the requested Code to lowercase using ToLower() to ensure case-insensitive comparison.
             * 1.4. Pass the cancellationToken to the AnyAsync method to allow for task cancellation if needed.
             * 1.5. Await the asynchronous call to AnyAsync and return true if a match is found, or false if not.
             */
            return await _appDbContext.Markets
                .AnyAsync(m => m.Code.ToLower() == request.Code.ToLower(), cancellationToken);
        }
    }
}
