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
        // Fetch markets from the database and project them into MarketDetailsDto
        var markets = await _context.Markets
            .Select(m => new MarketDetailsDto
            {
                Id = m.Id,  // Map the market Id
                Name = m.Name,  // Map the market Name
                Code = m.Code,  // Map the market Code
                LongMarketCode = m.LongMarketCode,  // Map the LongMarketCode
                Region = m.Region.ToString(),  // Convert the Region enum to string
                SubRegion = m.SubRegion.ToString(),  // Convert the SubRegion enum to string
                MarketSubGroups = m.MarketSubGroups
                    .Select(sg => new MarketSubGroupDto
                    {
                        SubGroupId = sg.SubGroupId,  // Map SubGroupId
                        SubGroupName = sg.SubGroupName,  // Map SubGroupName
                        SubGroupCode = sg.SubGroupCode  // Map SubGroupCode
                    }).ToList()  // Convert MarketSubGroups to List<MarketSubGroupDto>
            })
            // Filter markets based on the provided name in the request
            .Where(m => m.Name.Contains(request.Name))  // Case-sensitive search
            .ToListAsync(cancellationToken); // Asynchronously execute the query with cancellation support

        return markets;  // Return the list of MarketDetailsDto
    }
}
