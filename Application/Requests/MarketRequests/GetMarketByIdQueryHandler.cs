using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class GetMarketByIdQueryHandler : IRequestHandler<GetMarketByIdQuery, Market>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: GetMarketByIdQueryHandler
         * Initializes the handler with the database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public GetMarketByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Retrieves a market entry from the database based on the provided market ID.
         * 
         * Parameters:
         * - request: GetMarketByIdQuery - The query object containing the market ID to be retrieved.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<Market>: Asynchronously returns the Market entity with the specified ID or null if not found.
         */
        public async Task<Market> Handle(GetMarketByIdQuery request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Use the AppDbContext to access the Markets DbSet.
             * 2. Call the `FindAsync` method on the Markets DbSet with the provided market ID from the request.
             * 3. Pass the market ID as an object array to `FindAsync`.
             * 4. Include the cancellationToken in the `FindAsync` call to handle cancellation.
             * 5. Await the asynchronous call to retrieve the market entity with the specified ID.
             * 6. Return the retrieved Market entity, or null if the entity is not found.
             */
            return await _context.Markets.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
