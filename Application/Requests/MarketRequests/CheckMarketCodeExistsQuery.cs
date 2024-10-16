using MediatR;

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
