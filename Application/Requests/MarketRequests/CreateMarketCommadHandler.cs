using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, int>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: CreateMarketCommandHandler
         * Initializes the CreateMarketCommandHandler with the application's database context.
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
         * Handles the CreateMarketCommand to create a new market entry in the database.
         * 
         * Parameters:
         * - request: CreateMarketCommand - The command object containing details for creating a new market.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<int>: Asynchronously returns the ID of the newly created market entity.
         */
        public async Task<int> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Validate the SubRegion for the specified Region
            /*
             * LLD Steps:
             * 1.1. Call the static method IsValidSubRegionForRegion in the RegionSubRegionValidation class.
             * 1.2. Pass the requested Region and SubRegion as parameters.
             * 1.3. If the SubRegion is not valid for the provided Region, throw a ValidationException with a descriptive error message.
             */
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            // Step 2: Check if a market with the same name already exists in the database
            /*
             * LLD Steps:
             * 2.1. Use the _context to access the Markets DbSet.
             * 2.2. Call the FirstOrDefaultAsync method to check if any market entry has the requested Name.
             * 2.3. Pass the Name and the cancellationToken as parameters to ensure the operation can be canceled if needed.
             * 2.4. If an existing market with the same name is found, throw a ValidationException with a descriptive error message.
             */
            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name, cancellationToken);

            if (existingMarketByName != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
                throw validationError;
            }

            // Step 3: Check if a market with the same code already exists in the database
            /*
             * LLD Steps:
             * 3.1. Use the _context to access the Markets DbSet.
             * 3.2. Call the FirstOrDefaultAsync method to check if any market entry has the requested Code.
             * 3.3. Pass the Code and the cancellationToken as parameters to ensure the operation can be canceled if needed.
             * 3.4. If an existing market with the same code is found, throw a ValidationException with a descriptive error message.
             */
            var existingMarketByCode = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (existingMarketByCode != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
                throw validationError;
            }

            // Step 4: Create a new Market entity with the provided details
            /*
             * LLD Steps:
             * 4.1. Initialize a new instance of the Market entity.
             * 4.2. Assign the request's Name, Code, LongMarketCode, Region, and SubRegion values to the corresponding properties of the Market entity.
             */
            var market = new Market
            {
                Name = request.Name,
                Code = request.Code,
                LongMarketCode = request.LongMarketCode,
                Region = request.Region,
                SubRegion = request.SubRegion
            };

            // Step 5: Add the new Market entity to the database context and save changes
            /*
             * LLD Steps:
             * 5.1. Add the newly created market entity to the Markets DbSet using _context.Markets.Add.
             * 5.2. Call SaveChangesAsync on the _context, passing the cancellationToken to save the changes to the database.
             * 5.3. Await the result to ensure the market is saved successfully.
             */
            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 6: Return the ID of the newly created market entity
            return market.Id;
        }
    }
}
