using Domain.Model;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Markets.Commands
{
    public class CreateMarketCommandHandler : IRequestHandler<CreateMarketCommand, string>
    {
        private readonly IMarketRepository _marketRepository;

        public CreateMarketCommandHandler(IMarketRepository marketRepository)
        {
            _marketRepository = marketRepository;
        }

        public async Task<string> Handle(CreateMarketCommand request, CancellationToken cancellationToken)
        {
            // Check if the MarketName already exists
            var existingMarketName = await _marketRepository.GetMarketByNameAsync(request.MarketName);
            if (existingMarketName != null)
            {
                return "Market Name already exists. Please enter a new one.";
            }

            // Check if the MarketCode already exists
           /* var existingMarketCode = await _marketRepository.GetMarketByCodeAsync(request.MarketCode);
            if (existingMarketCode != null)
            {
                return "Market Code already exists. Please enter a new one.";
            }*/

            // Create the Market object
            var market = new Market
            {
                MarketName = request.MarketName,
                MarketCode = request.MarketCode.ToUpper(),
                LongMarketCode = request.LongMarketCode,
             // CountryList = request.CountryList,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add the market to the repository
            await _marketRepository.AddMarketAsync(market);

            return "Market created successfully";
        }
    }
}
