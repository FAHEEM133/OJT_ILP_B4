using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{

    /**
     * @class GetMarketSubGroupByIdQueryHandler
     * Handles the retrieval of a specific MarketSubGroup by its ID from the database.
     */
    public class GetMarketSubGroupByIdQueryHandler : IRequestHandler<GetMarketSubGroupByIdQuery, MarketSubGroup>
    {
        private readonly AppDbContext _context;

        /**
         * Constructor
         * @param context AppDbContext - Injected database context for interacting with MarketSubGroup entities.
         */
        public GetMarketSubGroupByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the query to retrieve a MarketSubGroup by its ID.
         * @param request GetMarketSubGroupByIdQuery - Contains the SubGroupId to query.
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Queries the database for a MarketSubGroup matching the given SubGroupId, and includes the related Market entity if needed.
         * 
         * @return MarketSubGroup - Returns the MarketSubGroup if found, otherwise returns null.
         */

        public async Task<MarketSubGroup> Handle(GetMarketSubGroupByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.MarketSubGroups
                .Include(m => m.Market) 
                .FirstOrDefaultAsync(m => m.SubGroupId == request.SubGroupId, cancellationToken);
        }
    }
}
