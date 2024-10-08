using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Requests.MarketRequests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Domain.Enums.Domain.Enums;

public class SearchMarketQueryHandler : IRequestHandler<SearchMarketQuery, List<MarketDetailsDto>>
{
    private readonly AppDbContext _context;

    public SearchMarketQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MarketDetailsDto>> Handle(SearchMarketQuery request, CancellationToken cancellationToken)
    {
        var markets = await _context.Markets
            .Select(m => new MarketDetailsDto
            {
                MarketId = m.Id,
                MarketName = m.Name,
                MarketCode = m.Code,
                LongMarketCode = m.LongMarketCode,
                Region = m.Region.ToString(),
                SubRegion  = m.SubRegion.ToString(),
                MarketSubGroups = m.MarketSubGroups
            .Select(sg => new MarketSubGroupDto
            {
                SubGroupId = sg.SubGroupId,
                SubGroupName = sg.SubGroupName,
                SubGroupCode = sg.SubGroupCode
            }).ToList() // Map the list of MarketSubGroup to MarketSubGroupDto
            })
    .Where(m => m.MarketName.Contains(request.Name))
    .ToListAsync(cancellationToken);

        return markets;
    }
}
