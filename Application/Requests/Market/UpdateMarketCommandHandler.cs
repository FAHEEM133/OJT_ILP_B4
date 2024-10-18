using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Application.Requests.MarketRequests;

public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, object>
{
    private readonly AppDbContext _context;

    public UpdateMarketCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
    {
        // Step 1: Fetch the existing market by ID and include its subgroups in the query.
        var existingMarket = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (existingMarket == null)
        {
            throw new ValidationException($"Market with ID {request.Id} not found.");
        }

        // Step 2: Validate SubRegion for the specified Region.
        if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
        {
            throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
        }

        // Step 3: Check for an existing market with the same name and code.
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

        // Step 4: Update the market entity with the new values.
        existingMarket.Name = request.Name ?? existingMarket.Name;
        existingMarket.Code = request.Code ?? existingMarket.Code;
        existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
        existingMarket.Region = request.Region;
        existingMarket.SubRegion = request.SubRegion;

        var existingSubGroups = existingMarket.MarketSubGroups.ToList();

        // Step 5: Remove subgroups marked as deleted in the request.
        var removedSubgroups = existingSubGroups
            .Where(existing => request.MarketSubGroups
                .Any(req => req.SubGroupId == existing.SubGroupId && req.IsDeleted))
            .ToList();
        if (removedSubgroups.Count > 0)
        {
            _context.MarketSubGroups.RemoveRange(removedSubgroups);
        }

        // Step 6: Add new subgroups where SubGroupId is null.
        var newSubGroups = request.MarketSubGroups
            .Where(req => req.SubGroupId == null)
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

        // Step 7: Update existing subgroups where IsEdited is true.
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

        // Step 8: Save changes to the database.
        await _context.SaveChangesAsync(cancellationToken);

        // Fetch and return the updated market.
        var updatedMarket = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        return updatedMarket.Id;
    }
}
