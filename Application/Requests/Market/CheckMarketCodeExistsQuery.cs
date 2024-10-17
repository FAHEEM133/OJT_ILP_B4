using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Requests.MarketRequests;

/// <summary>
/// Query to check if a market code exists in the database.
/// </summary>
public class CheckMarketCodeExistsQuery : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the market code that needs to be checked for existence.
    /// </summary>
    public string Code { get; set; }
}

/// <summary>
/// Handles CheckMarketCodeExistQuery to check whether the provided market code already exists on the database
/// </summary>
/// <remarks>
/// Initializes with application dbcontext
/// </remarks>
/// <param name="appDbContext"></param>
public class CheckMarketCodeExistsQueryHandler(AppDbContext appDbContext) : IRequestHandler<CheckMarketCodeExistsQuery, bool>
{
    /// <summary>
    /// The application's database context used to interact with the database.
    /// </summary>
    private readonly AppDbContext _appDbContext = appDbContext;

    /// <summary>
    /// Handles the <see cref="CheckMarketCodeExistsQuery"/> to check whether the market code already exists in the market
    /// </summary>
    /// <param name="request">The query object containing the code that needs to be checked</param>
    /// <param name="token">Token for handling the cancel operation</param>
    /// <returns>A Task that asynchronusly return a bool (true if the code exists and false if it does not exist)</returns>
    public async Task<bool> Handle(CheckMarketCodeExistsQuery query, CancellationToken token)
    {
        /// Step
        /// 1 Use the _appDbContext to access the Markets DbSet.
        /// 2. Use the AnyAsync method to check for any market entry where the code matches the requested code in a case-insensitive manner.
        /// 3. Convert both the database Name and the requested Name to lowercase using ToLower() to ensure case-insensitive comparison.
        /// 4. Pass the cancellationToken to the AnyAsync method to allow for task cancellation if needed.
        /// 5. Await the asynchronous call to AnyAsync and return true if a match is found, or false if not.
        return await _appDbContext.Markets.AnyAsync(m => m.Code.ToLower() == query.Code.ToLower(), token);
       
    }
}
