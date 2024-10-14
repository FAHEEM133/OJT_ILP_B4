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
            
            var market = await _context.Markets
                .Include(m => m.MarketSubGroups)   
                .FirstOrDefaultAsync(m => m.Id == request.MarketId, cancellationToken);  

            
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
}
