using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.MarketRequests
{
    public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, (List<Market> Markets, int TotalCount)>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: GetAllMarketsQueryHandler
         * Initializes the GetAllMarketsQueryHandler with the application's database context.
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
         * Handles the GetAllMarketsQuery to fetch a paginated list of markets and the total count of available markets.
         * 
         * Parameters:
         * - request: GetAllMarketsQuery - The query object containing pagination details.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<(List<Market> Markets, int TotalCount)>: Asynchronously returns a tuple containing a list of markets and the total market count.
         */
        public async Task<(List<Market> Markets, int TotalCount)> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
        {
            /*
             * 1. Retrieve the total count of available markets in the database.
             * 2. Fetch the list of markets based on the page number and page size specified in the request.
             * 3. Include the associated MarketSubGroups for each market.
             * 4. Return the list of markets along with the total count.
             */

            var totalCount = await _context.Markets.CountAsync(cancellationToken);

            var markets = await _context.Markets
                                        .Include(m => m.MarketSubGroups)
                                        .Skip((request.PageNumber - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .ToListAsync(cancellationToken);

            return (markets, totalCount);
        }
    }
}
