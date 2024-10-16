using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Handles the creation of a new market entry, including validation of market details and associated subgroups.
/// </summary>
public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, object>
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMarketCommandHandler"/> class with the provided database context.
    /// </summary>
    /// <param name="context">The application's database context used to interact with the Markets table.</param>
    public CreateMarketCommandHandler(AppDbContext context)
    {
        _context = context;
    }

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

        
        return market.Id;
    }
}
