using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, List<Market>>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: GetAllMarketsQueryHandler
         * Initializes the handler with the database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public GetAllMarketsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Retrieves all market entries from the database.
         * 
         * Parameters:
         * - request: GetAllMarketsQuery - The query object used for fetching all markets.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<List<Market>>: Asynchronously returns a list of all Market entities from the database.
         */
        public async Task<List<Market>> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Access the Markets DbSet from the AppDbContext.
             * 2. Fetch all market entries from the database asynchronously using the `ToListAsync` method.
             * 3. Pass the cancellationToken to the `ToListAsync` method to allow for operation cancellation if needed.
             * 4. Return the list of all Market entities retrieved from the database.
             */
            return await _context.Markets.ToListAsync(cancellationToken);
        }
    }
}
