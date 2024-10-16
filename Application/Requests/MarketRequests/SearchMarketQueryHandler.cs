using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Enums.Domain.Enums;


namespace Application.Requests.MarketRequests;

public class SearchMarketQueryHandler : IRequestHandler<SearchMarketQuery, List<MarketDetailsDto>>
{

    private readonly AppDbContext _context;

    public SearchMarketQueryHandler(AppDbContext context)
    {
        _context = context;
    }


    public async Task<List<MarketDetailsDto>> Handle(SearchMarketQuery request, CancellationToken cancellationToken)
    {
        
        var marketsQuery = _context.Markets
            .Where(m => m.Name.Contains(request.SearchText)
                     || m.Code.Contains(request.SearchText)
                     || m.LongMarketCode.Contains(request.SearchText))
            .Select(m => new MarketDetailsDto
            {
                Id = m.Id,
                Name = m.Name,
                Code = m.Code,
                LongMarketCode = m.LongMarketCode,
                Region = m.Region.ToString(),
                SubRegion = m.SubRegion.ToString(),
                MarketSubGroups = m.MarketSubGroups
                    .Select(sg => new MarketSubGroupDto
                    {
                        SubGroupId = sg.SubGroupId,
                        SubGroupName = sg.SubGroupName,
                        SubGroupCode = sg.SubGroupCode
                    }).ToList()
            });

        var markets = await marketsQuery.ToListAsync(cancellationToken);

        return markets;
    }

}
