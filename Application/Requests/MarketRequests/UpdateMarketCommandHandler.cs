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
    public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, int>
    {
        private readonly AppDbContext _context;

        public UpdateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Retrieve the existing market entry by ID
            var existingMarket = await _context.Markets
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (existingMarket == null)
            {
                throw new ValidationException($"Market with ID {request.Id} not found.");
            }

            // Step 2: Validate the SubRegion for the specified Region
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            // Step 3: Check for any existing market with the same name but different ID
            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != request.Id, cancellationToken);

            if (existingMarketByName != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
                throw validationError;
            }

            // Step 4: Check for any existing market with the same code but different ID
            var existingMarketByCode = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.Code && m.Id != request.Id, cancellationToken);

            if (existingMarketByCode != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
                throw validationError;
            }

            // Step 5: Update the existing market entry with the new values
            existingMarket.Name = request.Name;
            existingMarket.Code = request.Code;
            existingMarket.LongMarketCode = request.LongMarketCode;
            existingMarket.Region = request.Region;
            existingMarket.SubRegion = request.SubRegion;

            // Step 6: Save changes to the database
            _context.Markets.Update(existingMarket);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 7: Return the ID of the updated market entity
            return existingMarket.Id;
        }
    }
}
