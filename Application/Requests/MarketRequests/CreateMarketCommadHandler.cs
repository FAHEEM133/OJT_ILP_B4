using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MarketRequests
{
    /// <summary>
    /// Handles the creation of a new market entry, including validation of market details and associated subgroups.
    /// </summary>
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
        /// <summary>
        /// Handles the creation of a new market, ensuring validation of market and subgroup details.
        /// </summary>
        /// <param name="request">The <see cref="CreateMarketCommand"/> containing the market's details and optional subgroups.</param>
        /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
        /// <returns>A response object containing the newly created market and its subgroups.</returns>
        public async Task<object> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            // LLD steps : 
            // 1. Validate the SubRegion for the specified Region
            // 2. Check if a market with the same name already exists
            // 3. Check if a market with the same code already exists
            // 4. Create a new Market entity
            // 5. Add subgroups to the Market, if they are provided and valid.
            // 6. Add the new Market to the database and save changes
            // 7. Return a response containing the created Market and its subgroups
        {
            _context = context;
        }

        /*
         * Method: Handle

            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name, cancellationToken);
         * Parameters:
         * - request: CreateMarketCommand - The command object containing details for creating a new market.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<object>: Asynchronously returns a response object containing the newly created market and its subgroups.
         */
        public async Task<object> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Validate the SubRegion for the specified Region
            /*
             * 1. Check if the SubRegion is valid for the given Region.
             * 2. Check if a market with the same name already exists in the database.
             * 3. Check if a market with the same code already exists in the database.
             * 4. Create a new Market entity with the provided details.
             * 5. Add any provided SubGroups to the Market entity.

            var market = new Market
            {
                Name = request.Name,
                Code = request.Code.ToUpper(),
                LongMarketCode = request.LongMarketCode.ToUpper(),
                Region = request.Region,
                SubRegion = request.SubRegion
            };


            if (request.MarketSubGroups != null && request.MarketSubGroups.Count > 0)
            {
                foreach (var subGroupDto in request.MarketSubGroups)
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
                Code = request.Code.ToUpper(),
                LongMarketCode = request.LongMarketCode.ToUpper(),
                Region = request.Region,
            
            return market.Id;
        }
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
                    SubGroupCode = sg.SubGroupCode
                }).ToList() 
            };

            // Return the formatted response object
            return response;
        }
    }
}
