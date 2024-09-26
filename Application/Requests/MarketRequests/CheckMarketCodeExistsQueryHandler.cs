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
    public class CheckMarketCodeExistsQueryHandler : IRequestHandler<CheckMarketCodeExistsQuery, bool>
    {
        private readonly AppDbContext _appDbContext;

        public CheckMarketCodeExistsQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<bool> Handle(CheckMarketCodeExistsQuery request, CancellationToken cancellationToken)
        {
            // Convert both sides to lowercase for case-insensitive comparison
            return await _appDbContext.Markets
                .AnyAsync(m => m.Code.ToLower() == request.Code.ToUpper(), cancellationToken);
        }

    }
}
