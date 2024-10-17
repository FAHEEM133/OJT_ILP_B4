using Application.DTOs;
using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    using MediatR;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using Domain.Enums;
    using Domain.Enums.Domain.Enums;

    namespace Application.Requests.MarketRequests
    {
        public class FilterMarketsQueryHandler : IRequestHandler<FilterMarketsQuery, List<MarketDetailsDto>>
        {
            private readonly AppDbContext _appDbContext;

            public FilterMarketsQueryHandler(AppDbContext context)
            {
                _appDbContext = context;
            }

            public async Task<List<MarketDetailsDto>> Handle(FilterMarketsQuery request, CancellationToken cancellationToken)
            {
                // Query the database
                var query = _appDbContext.Markets
                    .Include(m => m.MarketSubGroups)
                    .AsQueryable();

                // Apply filter for multiple Regions if provided
                if (!string.IsNullOrEmpty(request.Regions))
                {
                    // Split the string into a list of integers (Region enum values)
                    var regionIds = request.Regions.Split(',')
                                                   .Select(int.Parse)
                                                   .Cast<Region>()
                                                   .ToList();

                    // Apply the filter for regions
                    query = query.Where(m => regionIds.Contains(m.Region));
                }

                // Project to DTO
                var result = await query
                    .Select(m => new MarketDetailsDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Code = m.Code,
                        LongMarketCode = m.LongMarketCode,
                        Region = m.Region.ToString(), // Convert Region enum to string
                        SubRegion = m.SubRegion.ToString(), // Assuming SubRegion is an enum, convert to string
                        MarketSubGroups = m.MarketSubGroups.Select(sg => new MarketSubGroupDto
                        {
                            SubGroupId = sg.SubGroupId,
                            SubGroupName = sg.SubGroupName,
                            SubGroupCode = sg.SubGroupCode
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                return result;
            }
        }
    }
    

}

