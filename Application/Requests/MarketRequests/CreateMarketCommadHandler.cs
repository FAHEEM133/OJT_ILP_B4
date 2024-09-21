using Domain.Model;
using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var market = new Market
            {
                MarketName = request.MarketName,
                MarketCode = request.MarketCode,
                LongMarketCode = request.LongMarketCode
            };

            _context.Markets.Add(market);
            await _context.SaveChangesAsync(cancellationToken);

            return market.Id;
        }

    }
}
