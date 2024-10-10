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

    /**
     * @method Handle
     * 
     * Handles the incoming request to search for markets based on a name query. It retrieves 
     * a list of markets from the database, along with their related subgroups. The region and 
     * subregion enum values are converted to string equivalents and returned as a list of DTOs.
     * 
     * @param {SearchMarketQuery} request
     * The request object containing the `Name` to search for.
     * 
     * @param {CancellationToken} cancellationToken
     * Used to cancel the asynchronous operation if needed.
     * 
     * @returns {Task<List<MarketDetailsDto>>}
     * Returns a list of `MarketDetailsDto` populated with market and subgroup information.
     */
    public async Task<List<MarketDetailsDto>> Handle(SearchMarketQuery request, CancellationToken cancellationToken)
    {
        // Fetch markets and filter by name, code, or longMarketCode using the provided search text
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
