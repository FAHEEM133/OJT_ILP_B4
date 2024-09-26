using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class DeleteMarketByIdCommandHandler : IRequestHandler<DeleteMarketByIdCommand, bool>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: DeleteMarketByIdCommandHandler
         * Initializes the handler with the database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public DeleteMarketByIdCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Deletes a market entry from the database based on the provided market ID.
         * 
         * Parameters:
         * - request: DeleteMarketByIdCommand - The command object containing the market ID to be deleted.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<bool>: Asynchronously returns true if the market was successfully deleted, or false if not found.
         */
        public async Task<bool> Handle(DeleteMarketByIdCommand request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Use the AppDbContext to access the Markets DbSet.
             * 2. Call the `FindAsync` method on the Markets DbSet with the provided market ID from the request.
             * 3. Pass the market ID as an object array to `FindAsync`.
             * 4. Include the cancellationToken in the `FindAsync` call to handle cancellation.
             * 5. Await the asynchronous call to retrieve the market entity with the specified ID.
             * 6. If the market is not found, return false to indicate failure.
             * 7. If the market is found, remove it from the DbSet.
             * 8. Save changes asynchronously and return true to indicate success.
             */
            var market = await _context.Markets.FindAsync(new object[] { request.Id }, cancellationToken);

            if (market == null)
            {
                return false; // Market with the specified ID not found
            }

            _context.Markets.Remove(market); // Remove the found market
            await _context.SaveChangesAsync(cancellationToken); // Save changes to the database

            return true; // Deletion successful
        }
    }
}
