using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, object>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: UpdateMarketCommandHandler
         * Initializes the UpdateMarketCommandHandler with the application's database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public UpdateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Handles the UpdateMarketCommand to update an existing market entry in the database.
         * 
         * Parameters:
         * - request: UpdateMarketCommand - The command object containing updated market details.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<object>: Asynchronously returns the updated market entity and its subgroups.
         */
        public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            /*
             * 1. Check if the specified market exists in the database.
             * 2. Validate the provided Region and SubRegion.
             * 3. Validate that the new name and code are unique across other markets.
             * 4. Update the main fields of the market.
             * 5. Handle the MarketSubGroups: remove subgroups not in the request, add new subgroups, and update existing ones.
             * 6. Save the updated market and its subgroups to the database.
             * 7. Return a response containing the updated market details, including subgroups.
             */

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

            // Handle MarketSubGroups: Add, Update, Remove
            var existingSubGroups = existingMarket.MarketSubGroups.ToList();

            // Remove subgroups that are not in the request
            var subGroupsToRemove = existingSubGroups
                .Where(sg => !request.MarketSubGroups.Any(reqSg => reqSg.SubGroupId == sg.SubGroupId))
                .ToList();

            foreach (var subGroupToRemove in subGroupsToRemove)
            {
                _context.MarketSubGroups.Remove(subGroupToRemove);
            }

            foreach (var requestSubGroup in request.MarketSubGroups)
            {
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
                        SubGroupName = requestSubGroup.SubGroupName, // Convert to lowercase
                        SubGroupCode = requestSubGroup.SubGroupCode, // Convert to lowercase
                        MarketId = existingMarket.Id // Properly set the MarketId
                    };
                    _context.MarketSubGroups.Add(newSubGroup);
                }
            }

            // Save changes
            await _context.SaveChangesAsync(cancellationToken);

            // Reload the market with updated subgroups to return the correct response
            var updatedMarket = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            // Return updated market and subgroups data
            return new
            {
                id = updatedMarket.Id,
                name = updatedMarket.Name,
                code = updatedMarket.Code,
                longMarketCode = updatedMarket.LongMarketCode,
                region = (int)updatedMarket.Region,
                subRegion = (int)updatedMarket.SubRegion,
                marketSubGroups = updatedMarket.MarketSubGroups.Select(sg => new
                {
                    subGroupId = sg.SubGroupId,
                    subGroupName = sg.SubGroupName,
                    subGroupCode = sg.SubGroupCode,
                    marketId = sg.MarketId,
                    marketCode = updatedMarket.Code
                }).ToList()
            };
        }
    }
}
