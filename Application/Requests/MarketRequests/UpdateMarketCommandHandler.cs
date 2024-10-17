using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Handles the update operation for an existing market entity.
/// This class implements IRequestHandler to manage the UpdateMarketCommand request.
/// </summary>
public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, object>
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes the UpdateMarketCommandHandler with the application's database context.
    /// </summary>
    /// <param name="context">The application's database context used to interact with the database.</param>
    public UpdateMarketCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the UpdateMarketCommand to update an existing market entry in the database.
    /// </summary>
    /// <param name="request">The command object containing details for updating a market.</param>
    /// <param name="cancellationToken">Token for handling operation cancellation.</param>
    /// <returns>Asynchronously returns a response object containing the updated market and its subgroups.</returns>
    /// <exception cref="ValidationException">Thrown if validation errors occur, such as duplicate names or invalid subregions.</exception>
    public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
    {
        //LLD Steps : 
        // 1. Fetch the existing market by ID and include its subgroups in the query.
        // 2. Handle case when the market is not found, Throws an exception if the market does not exist.
        // 3. Validate SubRegion for the specified Region
        // 4. Check for an existing market with the same name, if the name is updated. Throws an exception if such a market exists.
        // 5. Check for an existing market with the same code, if the code is updated. Throws an exception if such a market exists.
        // 6. Updates the market entity with the new values. If no new value is provided, retains the existing value.
        // 7. Remove subgroups that are no longer present in the request
        // 8. Update or add subgroups provided in the request. Validates each SubGroupCode using SubGroupValidation.
        // 9. Save changes to the database.

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


        if (request.Name != null && existingMarket.Name != request.Name)
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


        existingMarket.Id = request.Id;
        existingMarket.Name = request.Name ?? existingMarket.Name;
        existingMarket.Code = request.Code ?? existingMarket.Code;
        existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
        existingMarket.Region = request.Region;
        existingMarket.SubRegion = request.SubRegion;



        var existingSubGroups = existingMarket.MarketSubGroups.ToList();

        
        var subGroupsToRemove = existingSubGroups
            .Where(sg => !request.MarketSubGroups.Any(reqSg => reqSg.SubGroupId == sg.SubGroupId))
            .ToList();

        foreach (var subGroupToRemove in subGroupsToRemove)
        {
            _context.MarketSubGroups.Remove(subGroupToRemove);
        }

        
        foreach (var requestSubGroup in request.MarketSubGroups)
        {
            if (!SubGroupValidation.IsValidSubGroupCode(requestSubGroup.SubGroupCode))
            {
                throw new ValidationException($"SubGroupCode {requestSubGroup.SubGroupCode} is invalid. It must be a single alphanumeric character.");
            }

            var existingSubGroup = existingSubGroups
                .FirstOrDefault(sg => sg.SubGroupId == requestSubGroup.SubGroupId);

            if (existingSubGroup != null)
            {
                
                existingSubGroup.SubGroupName = requestSubGroup.SubGroupName;
                existingSubGroup.SubGroupCode = requestSubGroup.SubGroupCode;
                existingSubGroup.MarketId = existingMarket.Id;
            }
            else
            {
                
                var newSubGroup = new MarketSubGroup
                {
                    SubGroupName = requestSubGroup.SubGroupName, 
                    SubGroupCode = requestSubGroup.SubGroupCode, 
                    MarketId = existingMarket.Id 
                };
                _context.MarketSubGroups.Add(newSubGroup);
            }
        }


        await _context.SaveChangesAsync(cancellationToken);


        var updatedMarket = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);


        return updatedMarket.Id;
    }
}