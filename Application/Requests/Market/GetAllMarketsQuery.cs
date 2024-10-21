using Application.DTOs;
using Domain.Enums;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Query to fetch all markets with pagination.
/// </summary>
public class GetAllMarketsQuery : IRequest<(List<MarketDetailsDto> Markets, int TotalCount)>
{
    /// <summary>
    /// Gets or sets the page number for the markets to be fetched. Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of markets to fetch per page. Defaults to 10.
    /// </summary>
    public int PageSize { get; set; } = 10;
    /// <summary>
    /// search text to filter the markets by name, code, or longMarketCode.
    /// If provided, the query will return markets matching this text.
    /// </summary>
    public string? SearchText { get; set; }

    public string? Regions { get; set; }
}

/// <summary>
/// Handles the query to fetch a paginated list of markets and the total count of available markets.
/// </summary>
public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, (List<MarketDetailsDto> Markets, int TotalCount)>
{
    /// <summary>
    /// The application's database context used to interact with the database.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes the <see cref="GetAllMarketsQueryHandler"/> with the application's database context.
    /// </summary>
    /// <param name="context">The application's database context used to interact with the database.</param>
    public GetAllMarketsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the <see cref="GetAllMarketsQuery"/> to fetch a paginated list of markets and the total count of available markets.
    /// </summary>
    /// <param name="request">The query object containing pagination details.</param>
    /// <param name="cancellationToken">Token for handling operation cancellation.</param>
    /// <returns>A Task that asynchronously returns a tuple containing a list of markets and the total market count.</returns>
    public async Task<(List<MarketDetailsDto> Markets, int TotalCount)> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Markets.AsQueryable();

        // If search text is provided, filter by name, code, or longMarketCode
        if (!string.IsNullOrEmpty(request.SearchText))
        {
            query = query.Where(m => m.Name.Contains(request.SearchText)
                                  || m.Code.Contains(request.SearchText)
                                  || m.LongMarketCode.Contains(request.SearchText));
        }

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
        /// Step 1: Retrieve the total count of available markets in the database.
        /// Step 2: Fetch the list of markets based on the page number and page size specified in the request.
        /// Step 3: Include the associated MarketSubGroups for each market.
        /// Step 4: Return the list of markets along with the total count.

        var totalCount = await query.CountAsync(cancellationToken);

        var markets = await query
                   .Include(m => m.MarketSubGroups)
                   .Skip((request.PageNumber - 1) * request.PageSize)
                   .Take(request.PageSize)
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
                   })
                   .ToListAsync(cancellationToken);

        return (markets, totalCount);
    }
}