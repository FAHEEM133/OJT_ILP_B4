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
             * 2.1. Call the IsValidSubRegionForRegion method from RegionSubRegionValidation to ensure the SubRegion belongs to the Region.
             * 2.2. Pass the Region and SubRegion provided in the request to the validation method.
             * 2.3. If the validation fails, throw a ValidationException indicating the mismatch.
             */
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            // Step 3: Validate and Update Market Name and Code
            /*
             * 3.1. If the requested Name differs from the current market Name, check for duplicates:
             *      - Use _context to search for a market with the same Name but a different ID.
             * 3.2. If a duplicate Name is found, throw a ValidationException.
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
             * 3.3. Similarly, if the requested Code differs from the current market Code, check for duplicates:
             *      - Use _context to search for a market with the same Code but a different ID.
             * 3.4. If a duplicate Code is found, throw a ValidationException.
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

            // Step 4: Update the Market entity fields
            /*
             * 4.1. Assign the new values to the Name, Code, LongMarketCode, Region, and SubRegion if provided; otherwise, retain the existing values.
             */
            existingMarket.Id = request.Id;
            existingMarket.Name = request.Name ?? existingMarket.Name;
            existingMarket.Code = request.Code ?? existingMarket.Code;
            existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
            existingMarket.Region = request.Region;
            existingMarket.SubRegion = request.SubRegion;


            // Step 5: Manage MarketSubGroups - Add, Update, Remove
            /*
             * 5.1. Fetch the existing MarketSubGroups associated with the current market.
             * 5.2. Identify subgroups that need to be removed by comparing them against the provided subgroups in the request.
             * 5.3. For each requested subgroup:
             *      - Check for its validity using the SubGroupValidation class.
             *      - Either update the existing subgroup or add a new one if it does not exist in the database.
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

            // Step 6: Persist changes to the database
            /*
             * 6.1. Call _context.SaveChangesAsync() to save all changes made to the database.
             * 6.2. Pass the cancellationToken to ensure the operation can be cancelled.
             */
            await _context.SaveChangesAsync(cancellationToken);

            // Step 7: Reload the updated Market entity with its subgroups
            /*
             * 7.1. Fetch the updated market entity from the database.
             * 7.2. Include its associated subgroups to return a complete response.
             */
            var updatedMarket = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            // Step 8: Return the updated market data
            /*
             * 8.1. Return an object containing the updated market details and its associated subgroups.
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