using Application.DTOs;
using Domain.Enums.Domain.Enums;
using Domain.Enums;
using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.MarketRequests
{
    public class GetMarketDetailsByIdQueryHandler : IRequestHandler<GetMarketDetailsByIdQuery, MarketDetailsDto>
    {
        private readonly AppDbContext _context;

        public GetMarketDetailsByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MarketDetailsDto> Handle(GetMarketDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch market with related subgroups
            var market = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);

            if (market == null) return null;  // Return null if market not found

            // Convert Region and SubRegion enum ids to string values
            var regionString = Enum.GetName(typeof(Region), market.Region);
            var subRegionString = Enum.GetName(typeof(SubRegion), market.SubRegion);

            // Convert Market to DTO
            var marketDetails = new MarketDetailsDto
            {
                MarketId = market.Id,
                MarketName = market.Name,
                MarketCode = market.Code,
                LongMarketCode = market.LongMarketCode,
                Region = regionString,      // Assign region string
                SubRegion = subRegionString, // Assign subregion string
                MarketSubGroups = market.MarketSubGroups.Select(subGroup => new MarketSubGroupDto
                {
                    SubGroupId = subGroup.SubGroupId,
                    SubGroupName = subGroup.SubGroupName,
                    SubGroupCode = subGroup.SubGroupCode
                }).ToList()
            };

            return marketDetails;
        }
    }
}
