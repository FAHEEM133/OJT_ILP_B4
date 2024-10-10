using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    /**
     * @class UpdateMarketCommandHandler
     * 
     * @description
     * Handles the request to update an existing market. Implements the `IRequestHandler`
     * interface from MediatR to manage the CQRS pattern. It performs validation, checks for 
     * uniqueness of the name and code, and updates the relevant fields of a market entity.
     * 
     * @implements IRequestHandler<UpdateMarketCommand, int>
     * The handler responds to the `UpdateMarketCommand` and returns the updated market's ID.
     * 
     * @dependencies
     * - `AppDbContext`: Injected to access the database.
     * - `RegionSubRegionValidation`: Used to validate the region and subregion relationship.
     * 
     * @methods
     * - `Handle`: Main method that handles the command and updates the market details.
     */
    public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, int>
    {
        private readonly AppDbContext _context;

        /**
         * @constructor
         * 
         * @param {AppDbContext} context
         * Injects the `AppDbContext` to access the database for updating market data.
         */
        public UpdateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * @method Handle
         * 
         * Handles the incoming request to update a market. It validates the provided data, checks for
         * any existing market with the same name or code, and updates the market in the database.
         * 
         * @param {UpdateMarketCommand} request
         * The command object containing the market details to be updated.
         * 
         * @param {CancellationToken} cancellationToken
         * Used to cancel the asynchronous request if needed.
         * 
         * @returns {Task<int>}
         * Returns the ID of the updated market entity.
         * 
         * @errorHandling
         * - Throws `ValidationException` if the market is not found.
         * - Throws `ValidationException` if the region-subregion relationship is invalid.
         * - Throws `ValidationException` if a market with the same name or code already exists.
         */
        public async Task<int> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            var existingMarket = await _context.Markets
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
                    var validationError = new ValidationException(new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
                    throw validationError;
                }
            }

            if (request.Code != null && existingMarket.Code != request.Code)
            {
                var existingMarketByCode = await _context.Markets
                    .FirstOrDefaultAsync(m => m.Code == request.Code && m.Id != request.Id, cancellationToken);

                if (existingMarketByCode != null)
                {
                    var validationError = new ValidationException(new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
                    throw validationError;
                }
            }

            if (request.Name != null)
                existingMarket.Name = request.Name;

            if (request.Code != null)
                existingMarket.Code = request.Code;

            if (request.LongMarketCode != null)
                existingMarket.LongMarketCode = request.LongMarketCode;

            existingMarket.Region = request.Region;
            existingMarket.SubRegion = request.SubRegion;

            _context.Markets.Update(existingMarket);
            await _context.SaveChangesAsync(cancellationToken);

            return existingMarket.Id;
        }
    }
}
