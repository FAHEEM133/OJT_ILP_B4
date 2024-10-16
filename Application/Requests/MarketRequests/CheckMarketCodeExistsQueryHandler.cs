using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Handles the query to check if a market code already exists in the database.
/// </summary>
public class CheckMarketCodeExistsQueryHandler : IRequestHandler<CheckMarketCodeExistsQuery, bool>
{
    /// <summary>
    /// The application's database context used to interact with the database.
    /// </summary>
    private readonly AppDbContext _appDbContext;

    /// <summary>
    /// Initializes the <see cref="CheckMarketCodeExistsQueryHandler"/> with the application's database context.
    /// </summary>
    /// <param name="appDbContext">The application's database context used to interact with the database.</param>
    public CheckMarketCodeExistsQueryHandler(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    /// <summary>
    /// Handles the <see cref="CheckMarketCodeExistsQuery"/> to check if a market code already exists in the database.
    /// </summary>
    /// <param name="request">The query object containing the market code to be checked.</param>
    /// <param name="cancellationToken">Token for handling operation cancellation.</param>
    /// <returns>A Task that asynchronously returns true if the market code exists, otherwise false.</returns>
    public async Task<bool> Handle(CheckMarketCodeExistsQuery request, CancellationToken cancellationToken)
    {
        /// Step 1: Perform a case-insensitive comparison to check if the market code exists.
        /// 
        /// LLD Steps:
        /// 1.1. Use the _appDbContext to access the Markets DbSet.
        /// 1.2. Use the AnyAsync method to check for any market entry where the Code matches the requested code in a case-insensitive manner.
        /// 1.3. Convert both the database Code and the requested Code to lowercase using ToLower() to ensure case-insensitive comparison.
        /// 1.4. Pass the cancellationToken to the AnyAsync method to allow for task cancellation if needed.
        /// 1.5. Await the asynchronous call to AnyAsync and return true if a match is found, or false if not.

        return await _appDbContext.Markets
            .AnyAsync(m => m.Code.ToLower() == request.Code.ToLower(), cancellationToken);
    }
}
