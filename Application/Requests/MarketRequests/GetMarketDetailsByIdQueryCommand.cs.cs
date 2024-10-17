using Application.DTOs;
using Domain.Enums;
using Domain.Enums.Domain.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Requests.MarketRequests;

/// <summary>
/// Represents a query to retrieve the details of a market by its unique identifier.
/// </summary>
public class GetMarketDetailsByIdQuery : IRequest<MarketDetailsDto>
{
    /// <summary>
    /// Gets or sets the unique identifier of the market to retrieve its details.
    /// </summary>
    public int Id { get; set; }
}

/// <summary>
/// Handles the request to fetch market details by market ID.
/// Retrieves market information along with associated subgroups from the database
/// and returns the result as a <see cref="MarketDetailsDto"/>.
/// </summary>
public class GetMarketDetailsByIdQueryHandler : IRequestHandler<GetMarketDetailsByIdQuery, MarketDetailsDto>
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMarketDetailsByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="context">The application's database context used to query the Markets table.</param>
    public GetMarketDetailsByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Handles the query to fetch market details by ID, including associated subgroups.
    /// Converts region and subregion enum values into their string equivalents and returns them in the DTO.
    /// </summary>
    /// <param name="request">The request containing the MarketId.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="MarketDetailsDto"/> populated with market and subgroup information,
    /// or null if the market is not found.
    /// </returns>
    public async Task<MarketDetailsDto> Handle(GetMarketDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        
        var market = await _context.Markets
            .Include(m => m.MarketSubGroups)   
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);  

        
        if (market == null) return null;

        var regionString = Enum.GetName(typeof(Region), market.Region);
        var subRegionString = Enum.GetName(typeof(SubRegion), market.SubRegion);

        
        var marketDetails = new MarketDetailsDto
        {
            Id = market.Id,               
            Name = market.Name,           
            Code = market.Code,           
            LongMarketCode = market.LongMarketCode, 
            Region = regionString,              
            SubRegion = subRegionString,        
            MarketSubGroups = market.MarketSubGroups.Select(subGroup => new MarketSubGroupDto
            {
                SubGroupId = subGroup.SubGroupId,    
                SubGroupName = subGroup.SubGroupName, 
                SubGroupCode = subGroup.SubGroupCode  
            }).ToList()  
        };

        
        return marketDetails;
    }
}
