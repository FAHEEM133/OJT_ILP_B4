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
     * @class GetAllMarketSubGroupsQueryHandler
     * Handles the retrieval of all MarketSubGroups from the database. 
     */
    public class GetAllMarketSubGroupsQueryHandler : IRequestHandler<GetAllMarketSubGroupsQuery, List<MarketSubGroup>>
    {
        private readonly AppDbContext _context;

        /**
         * Constructor
         * @param context AppDbContext - Injected database context for interacting with MarketSubGroup entities.
         */
        public GetAllMarketSubGroupsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the query to retrieve all MarketSubGroups.
         * @param request GetAllMarketSubGroupsQuery - Contains any filters or criteria for querying MarketSubGroups (if needed).
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Fetches all MarketSubGroup records from the database and returns them as a list.
         * 
         * @return List<MarketSubGroup> - A list of all MarketSubGroups.
         */
        public async Task<List<MarketSubGroup>> Handle(GetAllMarketSubGroupsQuery request, CancellationToken cancellationToken)
        {
            return await _context.MarketSubGroups.ToListAsync(cancellationToken);
        }
    }
}
