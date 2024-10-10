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
    public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, object>
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
         * - Task<object>: Asynchronously returns the ID of the newly created market entity.
         */
        public async Task<object> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            /*
             * 1. Check if the SubRegion is valid for the given Region.
             * 2. Check if a market with the same name already exists in the database.
             * 3. Check if a market with the same code already exists in the database.
             * 4. Create a new Market entity with the provided details.
             * 5. Add any provided SubGroups to the Market entity.
             * 6. Add the new Market entity to the database context and save the changes.
             * 7. Return a response with the details of the newly created market.
             */

            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name, cancellationToken);

            if (existingMarketByName != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
                throw validationError;
            }

            var existingMarketByCode = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (existingMarketByCode != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
                throw validationError;
            }

            var market = new Market
            {
                Name = request.Name,
                Code = request.Code,
                LongMarketCode = request.LongMarketCode,
                Region = request.Region,
                SubRegion = request.SubRegion
            };

           
            if (request.MarketSubGroups != null && request.MarketSubGroups.Count > 0)
            {
                foreach (var subGroupDto in request.MarketSubGroups)
                {
                    var marketSubGroups = new MarketSubGroup
                    {
                        SubGroupName = subGroupDto.SubGroupName,
                        SubGroupCode = subGroupDto.SubGroupCode,
                        Market = market 
                    };
                    market.MarketSubGroups.Add(marketSubGroups);
                }
            }

            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new
            {
                Name = market.Name,
                Code = market.Code,
                LongMarketCode = market.LongMarketCode,
                Region = market.Region,
                SubRegion = market.SubRegion,
                MarketSubGroups = market.MarketSubGroups.Select(sg => new
                {
                    SubGroupName = sg.SubGroupName,
                    SubGroupCode = sg.SubGroupCode,
                    MarketCode = market.Code
                }).ToList()
            };

            return response;
        }
    }
}
