using Application.DTOs;
using Application.Validations;
using Domain.Enums;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using System.ComponentModel.DataAnnotations; 
using System.Threading;
using Domain.Enums.Domain.Enums;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Command to create a new market with the provided details.
/// </summary>
public class CreateMarketCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string LongMarketCode { get; set; }
    public Region Region { get; set; }
    public SubRegion SubRegion { get; set; }
    public List<MarketSubGroupDTO> MarketSubGroups { get; set; } = new List<MarketSubGroupDTO>();
}

/// <summary>
/// Handler for the CreateMarketCommand, responsible for creating a new market with validation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CreateMarketCommandHandler"/> class with the provided database context and validator.
/// </remarks>
/// <param name="context">The application's database context used to interact with the Markets table.</param>
/// <param name="validator">The validator to ensure the command is valid before handling.</param>
public class CreateMarketCommandHandler(AppDbContext context, IValidator<CreateMarketCommand> validator) : IRequestHandler<CreateMarketCommand, int>
{
    private readonly AppDbContext _context = context;
    private readonly IValidator<CreateMarketCommand> _validator = validator;

    /// <summary>
    /// Handles the creation of a new market, ensuring validation of market and subgroup details.
    /// </summary>
    /// <param name="request">The <see cref="CreateMarketCommand"/> containing the market's details and optional subgroups.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>The ID of the newly created market.</returns>
    public async Task<int> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Perform validation using the validator.
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            // Use FluentValidation.ValidationException for validation issues
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        // Check for unique subgroup codes and names within the request itself
        var duplicateSubGroupCodes = request.MarketSubGroups
            .GroupBy(sg => sg.SubGroupCode)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList();

        if (duplicateSubGroupCodes.Any())
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException(
                new ValidationResult($"SubGroupCode must be unique within the market.", new[] { "SubGroupCode" }), null, null);
        }

        var duplicateSubGroupNames = request.MarketSubGroups
            .GroupBy(sg => sg.SubGroupName)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key).ToList();

        if (duplicateSubGroupNames.Any())
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException(
                new ValidationResult($"SubGroupName must be unique within the market.", new[] { "SubGroupName" }), null, null);
        }

        // Step 2: Check if a market with the same name already exists in the database.
        var existingMarketByName = await _context.Markets
            .FirstOrDefaultAsync(m => m.Name == request.Name, cancellationToken);
        if (existingMarketByName != null)
        {
            // Use System.ComponentModel.DataAnnotations.ValidationException for DB-related validation issues
            throw new System.ComponentModel.DataAnnotations.ValidationException(
                new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
        }

        // Step 3: Check if a market with the same code already exists in the database.
        var existingMarketByCode = await _context.Markets
            .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);
        if (existingMarketByCode != null)
        {
            throw new System.ComponentModel.DataAnnotations.ValidationException(
                new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
        }

        // Step 4: Create a new Market entity with the provided details.
        var market = new Market
        {
            Name = request.Name,
            Code = request.Code.ToUpper(),
            LongMarketCode = request.LongMarketCode.ToUpper(),
            Region = request.Region,
            SubRegion = request.SubRegion
        };

        // Step 5: Add subgroups to the Market if they are provided and valid.
        if (request.MarketSubGroups != null && request.MarketSubGroups.Count > 0)
        {
            foreach (var subGroupDto in request.MarketSubGroups)
            {
                // Subgroup validation has already been handled by the validator.

                if(!subGroupDto.IsDeleted)
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
        }

        // Step 6: Add the new Market to the database and save changes.
        _context.Markets.Add(market);
        await _context.SaveChangesAsync(cancellationToken);

        // Step 7: Return the ID of the created market.
        return market.Id;
    }
}
