using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Application.Requests.MarketRequests;

/// <summary>
/// Query to check if a market name exists in the database.
/// </summary>
public class CheckMarketNameExistsQuery : IRequest<bool>
{
    /// <summary>
    /// Gets or sets the market name that needs to be checked for existence.
    /// </summary>
    public string Name { get; set; }
}
/// <summary>
/// Handler class for <see cref="CheckMarketNameExistsQuery"/>
/// </summary>
public class CheckMarketNameExistsQueryHandler(AppDbContext appDbContext) : IRequestHandler<CheckMarketNameExistsQuery, bool>
{
    /// <summary>
    /// Initialized application db context for interacting with the database.
    /// </summary>
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<bool> Handle(CheckMarketNameExistsQuery query,CancellationToken token)
    {
        /// LLD Steps:
        /// 1.1. Use the _appDbContext to access the Markets DbSet.
        /// 1.2. Use the AnyAsync method to check for any market entry where the Name matches the requested name in a case-insensitive manner.
        /// 1.3. Convert both the database Name and the requested Name to lowercase using ToLower() to ensure case-insensitive comparison.
        /// 1.4. Pass the cancellationToken to the AnyAsync method to allow for task cancellation if needed.
        /// 1.5. Await the asynchronous call to AnyAsync and return true if a match is found, or false if not.
        return await _appDbContext.Markets.AnyAsync(m=>m.Name.ToLower()==query.Name.ToLower());
       
    }

}
