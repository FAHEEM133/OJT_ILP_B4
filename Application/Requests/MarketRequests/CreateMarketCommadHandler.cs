using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System.ComponentModel.DataAnnotations;

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
            // Validate that the SubRegion is valid for the selected Region
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            var market = new Market
            {
                MarketName = request.MarketName,
                MarketCode = request.MarketCode,
                LongMarketCode = request.LongMarketCode,
                Region = request.Region,  // Assign Region
                SubRegion = request.SubRegion  // Assign SubRegion
            };

            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            return market.Id;
        }
    }
}
