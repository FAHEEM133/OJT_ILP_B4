using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MarketRequests
{
    public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, int>
    {
        private readonly AppDbContext _context;

        /* 
         * Constructor: CreateMarketCommandHandler
         * Initializes the handler with the database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public CreateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Processes the CreateMarketCommand to create a new market entry in the database.
         * 
         * Parameters:
         * - request: CreateMarketCommand - The command object containing details for creating a market (MarketName, MarketCode, LongMarketCode, Region, SubRegion).
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<int>: Asynchronously returns the ID of the newly created market.
         * 
         * Throws:
         * - ValidationException: Thrown if the provided SubRegion is not valid for the given Region.
         */
        public async Task<int> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Check if the provided SubRegion is valid for the selected Region.
             * 2. If invalid, throw a ValidationException with an appropriate error message.
             * 3. Create a new Market entity using the data provided in the CreateMarketCommand request.
             * 4. Assign the MarketName, MarketCode, LongMarketCode, Region, and SubRegion properties to the new Market entity.
             * 5. Add the newly created Market entity to the database context (_context).
             * 6. Save the changes to the database asynchronously using the provided cancellation token.
             * 7. Return the ID of the newly created Market entity.
             */

            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            var market = new Market
            {
                Name = request.Name,
                Code = request.Code,
                LongMarketCode = request.LongMarketCode,
                Region = request.Region,
                SubRegion = request.SubRegion
            };

            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            return market.Id;
        }
    }
}
