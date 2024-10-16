using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MarketRequests;

public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, object>
{
    /// <summary>
    /// 
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
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
            Code = request.Code.ToUpper(),
            LongMarketCode = request.LongMarketCode.ToUpper(),
            Region = request.Region,
            SubRegion = request.SubRegion
        };

        // Add any SubGroups if provided
        if (request.MarketSubGroups != null && request.MarketSubGroups.Count > 0)
        {
            foreach (var subGroupDto in request.MarketSubGroups)
            {

                if (!SubGroupValidation.IsValidSubGroupCode(subGroupDto.SubGroupCode))
                {
                    throw new ValidationException($"SubGroupCode {subGroupDto.SubGroupCode} is invalid. It must be a single alphanumeric character.");
                }

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

        /* var response = new
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
         };*/

        // Return the formatted response object
        return market.Id;
    }
}
