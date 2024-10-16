using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Handles the query to fetch a paginated list of markets and the total count of available markets.
/// </summary>
public class GetAllMarketsQueryHandler : IRequestHandler<GetAllMarketsQuery, (List<Market> Markets, int TotalCount)>
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
    public async Task<(List<Market> Markets, int TotalCount)> Handle(GetAllMarketsQuery request, CancellationToken cancellationToken)
    {
        /// Step 1: Retrieve the total count of available markets in the database.
        /// Step 2: Fetch the list of markets based on the page number and page size specified in the request.
        /// Step 3: Include the associated MarketSubGroups for each market.
        /// Step 4: Return the list of markets along with the total count.

        var totalCount = await _context.Markets.CountAsync(cancellationToken);

        var markets = await _context.Markets
                                    .Include(m => m.MarketSubGroups)
                                    .Skip((request.PageNumber - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync(cancellationToken);

        return (markets, totalCount);
    }
}
