using Application.DTOs;
using Application.Validations;
using Domain.Enums.Domain.Enums;
using Domain.Enums;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Command to update an existing market with new details, including name, code, long code, region, subregion, and subgroups.
/// </summary>
public class UpdateMarketCommand : IRequest<object>
{
    /// <summary>
    /// Gets or sets the unique identifier of the market to be updated.
    /// </summary>
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the market.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the code of the market.
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the long market code, a more detailed version of the market code.
    /// </summary>
    public string LongMarketCode { get; set; }

    /// <summary>
    /// Gets or sets the region to which the market belongs.
    /// </summary>
    public Region Region { get; set; }

    /// <summary>
    /// Gets or sets the subregion to which the market belongs.
    /// </summary>
    public SubRegion SubRegion { get; set; }

    /// <summary>
    /// Gets or sets the list of subgroups associated with the market.
    /// </summary>
    public List<MarketSubGroupDTO> MarketSubGroups { get; set; } = new List<MarketSubGroupDTO>();
}

/// <summary>
/// Handles the updating of an existing market, including validation for market name, code,
/// region, subregion, and management of market subgroups.
/// </summary>
/// <param name="context">The application's database context used to interact with the Markets table.</param>
public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, object>
{
    private readonly AppDbContext _context;

    public UpdateMarketCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the logic for updating a market and its subgroups.
    /// </summary>
    /// <param name="request">The <see cref="UpdateMarketCommand"/> containing the updated market details.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>An object containing the ID of the updated market.</returns>
    /// <exception cref="ValidationException">Thrown if the market, region, or subgroup validation fails.</exception>
    public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
    {
        // LLD steps : 
        // 1. Retrieve the existing market and its subgroups by ID, throw an exception if not found.
        // 2. Validate the SubRegion for the specified Region.
        // 3. Check if the market name is being updated and validate for uniqueness.
        // 4. Check if the market code is being updated and validate for uniqueness.
        // 5. Update market details such as Name, Code, LongMarketCode, Region, and SubRegion.
        // 6. Identify and remove any subgroups that are marked for deletion.
        // 7. Add any new subgroups that are not marked as deleted and have no SubGroupId.
        // 8. Update existing subgroups that are marked as edited.
        // 9. Save all changes to the database.
        // 10. Retrieve the updated market with subgroups and return its ID.
        var existingMarket = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (existingMarket == null)
        {
            throw new ValidationException($"Market with ID {request.Id} not found.");
        }

        if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
        {
            throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
        }

       
        if (request.Name != null && existingMarket.Name.ToLower() != request.Name.ToLower())
        {
            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != request.Id, cancellationToken);

            if (existingMarketByName != null)
            {
                throw new ValidationException("A market with this name already exists.");
            }
        }

        if (request.Code != null && existingMarket.Code != request.Code)
        {
            var existingMarketByCode = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.Code && m.Id != request.Id, cancellationToken);

            if (existingMarketByCode != null)
            {
                throw new ValidationException("A market with this code already exists.");
            }
        }

        existingMarket.Name = request.Name ?? existingMarket.Name;
        existingMarket.Code = request.Code ?? existingMarket.Code;
        existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
        existingMarket.Region = request.Region;
        existingMarket.SubRegion = request.SubRegion;

        var existingSubGroups = existingMarket.MarketSubGroups.ToList();

     
        var removedSubgroups = existingSubGroups
            .Where(existing => request.MarketSubGroups
                .Any(req => req.SubGroupId == existing.SubGroupId && req.IsDeleted))
            .ToList();
        if (removedSubgroups.Count > 0)
        {
            _context.MarketSubGroups.RemoveRange(removedSubgroups);
        }

       
        var newSubGroups = request.MarketSubGroups
            .Where(req => !req.IsDeleted && req.SubGroupId == null)
            .Select(req => new MarketSubGroup
            {
                SubGroupName = req.SubGroupName,
                SubGroupCode = req.SubGroupCode,
                MarketId = existingMarket.Id
            })
            .ToList();
        if (newSubGroups.Count > 0)
        {
            _context.MarketSubGroups.AddRange(newSubGroups);
        }

      
        var editedSubGroups = existingSubGroups
            .Where(existing => request.MarketSubGroups
                .Any(req => req.SubGroupId == existing.SubGroupId && req.IsEdited))
            .ToList();
        foreach (var existingSubGroup in editedSubGroups)
        {
            var requestSubGroup = request.MarketSubGroups
                .First(req => req.SubGroupId == existingSubGroup.SubGroupId);

            existingSubGroup.SubGroupName = requestSubGroup.SubGroupName;
            existingSubGroup.SubGroupCode = requestSubGroup.SubGroupCode;
        }

        
        await _context.SaveChangesAsync(cancellationToken);

        var updatedMarket = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        return updatedMarket.Id;
    }
}
