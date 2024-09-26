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
    public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            // Validate the SubRegion
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            // Check if a market with the same name already exists
            var existingMarketByName = await _context.Markets
                .FirstOrDefaultAsync(m => m.Name == request.Name, cancellationToken);

            if (existingMarketByName != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this name already exists.", new[] { "Name" }), null, null);
                throw validationError;
            }

            // Check if a market with the same code already exists
            var existingMarketByCode = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.Code, cancellationToken);

            if (existingMarketByCode != null)
            {
                var validationError = new ValidationException(new ValidationResult("A market with this code already exists.", new[] { "Code" }), null, null);
                throw validationError;
            }

            // Create a new Market entity
            var market = new Market
            {
                Name = request.Name,
                Code = request.Code,
                LongMarketCode = request.LongMarketCode,
                Region = request.Region,
                SubRegion = request.SubRegion
            };

            // Add the market to the context and save changes
            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            return market.Id;
        }
    }
}
