using Application.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Handles search queries for markets by search text, which could match the market's name, code, or long code.
/// Returns a list of market details, including associated subgroups.
/// </summary>
public class SearchMarketQueryHandler : IRequestHandler<SearchMarketQuery, List<MarketDetailsDto>>
{

    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchMarketQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application's database context used to query the Markets table.</param>
    public SearchMarketQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the search query to retrieve a list of markets that match the search text in their name, code, or long code.
    /// The returned market details include subgroups.
    /// </summary>
    /// <param name="request">The search query containing the text to search for in market fields.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A list of <see cref="MarketDetailsDto"/> objects that match the search criteria.</returns>
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
