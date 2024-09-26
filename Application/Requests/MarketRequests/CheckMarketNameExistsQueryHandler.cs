using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class CheckMarketNameExistsQueryHandler : IRequestHandler<CheckMarketNameExistsQuery, bool>
    {
        private readonly AppDbContext _appDbContext;

        public CheckMarketNameExistsQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> Handle(CheckMarketNameExistsQuery request, CancellationToken cancellationToken)
        {
            // Convert both sides to lowercase for case-insensitive comparison
            return await _appDbContext.Markets
                .AnyAsync(m => m.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        }

    }
}
