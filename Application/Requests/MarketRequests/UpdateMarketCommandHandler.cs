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
         * - request: UpdateMarketCommand - The command object containing details for updating a market.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<object>: Asynchronously returns the updated market details.
         */
        public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Fetch the existing Market entity by ID
            /*
             * LLD Steps:
             * 1.1. Use _context to access the Markets DbSet.
             * 1.2. Include the MarketSubGroups navigation property to ensure subgroups are loaded.
             * 1.3. Call FirstOrDefaultAsync to fetch the market by ID.
             * 1.4. If no market is found, throw a ValidationException with a descriptive error message.
             */
            var existingMarket = await _context.Markets
                .Include(m => m.MarketSubGroups) 
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (existingMarket == null)
            {
                throw new ValidationException($"Market with ID {request.Id} not found.");
            }

            // Step 2: Validate the Region and SubRegion
            /*
             * LLD Steps:
             * 2.1. Call the IsValidSubRegionForRegion method from the RegionSubRegionValidation class.
             * 2.2. Pass the requested Region and SubRegion as parameters.
             * 2.3. If the SubRegion is invalid for the provided Region, throw a ValidationException with a descriptive error message.
             */
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            // Step 3: Validate and update Market Name and Code
            /*
             * LLD Steps for Name validation:
             * 3.1. If the requested Name is different from the existing one, check for duplicates.
             * 3.2. Use _context to search the Markets DbSet for another market with the same Name but different ID.
             * 3.3. If a duplicate Name is found, throw a ValidationException.
             */
            if (request.Name != null && existingMarket.Name != request.Name)
            {
                var existingMarketByName = await _context.Markets
                    .FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != request.Id, cancellationToken);

                if (existingMarketByName != null)
                {
                    throw new ValidationException("A market with this name already exists.");
                }
            }

            /*
             * LLD Steps for Code validation:
             * 3.4. If the requested Code is different from the existing one, check for duplicates.
             * 3.5. Use _context to search the Markets DbSet for another market with the same Code but different ID.
             * 3.6. If a duplicate Code is found, throw a ValidationException.
             */
            if (request.Code != null && existingMarket.Code != request.Code)
            {
                var existingMarketByCode = await _context.Markets
                    .FirstOrDefaultAsync(m => m.Code == request.Code && m.Id != request.Id, cancellationToken);

                if (existingMarketByCode != null)
                {
                    throw new ValidationException("A market with this code already exists.");
                }
            }

            // Step 4: Update the Market entity's fields
            /*
             * LLD Steps:
             * 4.1. Update the Name, Code, LongMarketCode, Region, and SubRegion of the existing Market entity.
             * 4.2. If the request provides new values, assign them; otherwise, retain the current values.
             */
            existingMarket.Id = request.Id;
            existingMarket.Name = request.Name ?? existingMarket.Name;
            existingMarket.Code = request.Code ?? existingMarket.Code;
            existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
            existingMarket.Region = request.Region;
            existingMarket.SubRegion = request.SubRegion;
            

            // Step 5: Handle MarketSubGroups - Add, Update, Remove
            /*
             * LLD Steps:
             * 5.1. Fetch the current list of MarketSubGroups associated with the existing market.
             * 5.2. Identify and remove subgroups that are not present in the request.
             * 5.3. Iterate over the requested MarketSubGroups to either update existing subgroups or add new ones.
             */
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

            // Step 6: Save changes to the database
            /*
             * LLD Steps:
             * 6.1. Call SaveChangesAsync on the _context, passing the cancellationToken to save the changes.
             * 6.2. Await the result to ensure all updates are persisted to the database.
             */
            await _context.SaveChangesAsync(cancellationToken);

            // Step 7: Reload the updated Market entity with subgroups
            /*
             * LLD Steps:
             * 7.1. Fetch the updated market, including its subgroups, from the database.
             * 7.2. Return the updated market details in the response.
             */
            var updatedMarket = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            // Step 8: Return the updated market and subgroups data
            /*
             * LLD Steps:
             * 8.1. Return a response object containing the updated Market entity details and associated subgroups.
             */
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
                    marketId = sg.MarketId
                }).ToList()
            };
        }
    }
}
