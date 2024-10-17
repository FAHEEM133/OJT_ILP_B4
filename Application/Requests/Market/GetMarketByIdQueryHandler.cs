using Application.Requests.MarketRequests;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Handlers.MarketHandlers;

/// <summary>
/// Handles the retrieval of a specific market by its ID.
/// Queries the database to fetch the market along with its related subgroups, if available.
/// </summary>
public class GetMarketByIdHandler : IRequestHandler<GetMarketByIdQuery, Market>
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMarketByIdHandler"/> class.
    /// </summary>
    /// <param name="context">The application's database context used to query the Markets table.</param>
    public GetMarketByIdHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to retrieve a market by its ID, including its associated subgroups.
    /// </summary>
    /// <param name="request">The request containing the MarketId to retrieve the market.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>The <see cref="Market"/> object corresponding to the provided MarketId, or null if not found.</returns>
    public async Task<Market> Handle(GetMarketByIdQuery request, CancellationToken cancellationToken)
    {
        
        var market = await _context.Markets
            .Include(m => m.MarketSubGroups)
            .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);

        
        if (market == null)
        {
            return null;
        }

        return market;
    }
}
