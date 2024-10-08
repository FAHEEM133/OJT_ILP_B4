using Application.DTOs;
using Domain.Enums;
using Domain.Enums.Domain.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    /**
     * @class GetMarketDetailsByIdQueryHandler
     * 
     * @description
     * Handles the request to fetch market details by market ID. Implements the `IRequestHandler`
     * interface from MediatR to manage the CQRS pattern. Retrieves market information along with 
     * associated subgroups from the database.
     * 
     * @implements IRequestHandler<GetMarketDetailsByIdQuery, MarketDetailsDto>
     * The handler responds to the `GetMarketDetailsByIdQuery` and returns a `MarketDetailsDto`.
     * 
     * @dependencies
     * - `AppDbContext`: Injected to access the database.
     * 
     * @methods
     * - `Handle`: Main method that handles the query and fetches the market details.
     */
    public class GetMarketDetailsByIdQueryHandler : IRequestHandler<GetMarketDetailsByIdQuery, MarketDetailsDto>
    {
        private readonly AppDbContext _context;

        /**
         * @constructor
         * 
         * @param {AppDbContext} context
         * Injects the `AppDbContext` to access the database for fetching market data.
         */
        public GetMarketDetailsByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * @method Handle
         * 
         * Handles the incoming request to fetch market details by ID. It retrieves the market along with its 
         * related subgroups from the database. The region and subregion enum values are converted into their 
         * string equivalents and then returned as a DTO.
         * 
         * @param {GetMarketDetailsByIdQuery} request
         * The request object containing the `MarketId`.
         * 
         * @param {CancellationToken} cancellationToken
         * Used to cancel the asynchronous request if needed.
         * 
         * @returns {Task<MarketDetailsDto>}
         * Returns the `MarketDetailsDto` populated with market and subgroup information.
         * 
         * @errorHandling
         * If the market is not found, the method returns `null`.
         */
        public async Task<MarketDetailsDto> Handle(GetMarketDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            // Fetch the market along with related subgroups based on the provided market ID
            var market = await _context.Markets
                .Include(m => m.MarketSubGroups)   // Include the subgroups when fetching market data
                .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);  // Fetch market by ID

            // If market is not found, return null
            if (market == null) return null;

            // Convert the Region and SubRegion enums into their corresponding string representations
            var regionString = Enum.GetName(typeof(Region), market.Region);
            var subRegionString = Enum.GetName(typeof(SubRegion), market.SubRegion);

            // Map the Market entity to a DTO (Data Transfer Object)
            var marketDetails = new MarketDetailsDto
            {
                Id = market.Id,               // Assign market ID
                Name = market.Name,           // Assign market name
                Code = market.Code,           // Assign market code
                LongMarketCode = market.LongMarketCode, // Assign long market code
                Region = regionString,              // Assign string value of region enum
                SubRegion = subRegionString,        // Assign string value of subregion enum
                MarketSubGroups = market.MarketSubGroups.Select(subGroup => new MarketSubGroupDto
                {
                    SubGroupId = subGroup.SubGroupId,    // Assign subgroup ID
                    SubGroupName = subGroup.SubGroupName, // Assign subgroup name
                    SubGroupCode = subGroup.SubGroupCode  // Assign subgroup code
                }).ToList()  // Convert subgroups to list of DTOs
            };

            // Return the market details DTO
            return marketDetails;
        }
    }
}
