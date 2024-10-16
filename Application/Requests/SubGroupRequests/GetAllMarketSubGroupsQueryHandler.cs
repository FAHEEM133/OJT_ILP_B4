using Application.DTOs;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class GetAllMarketSubGroupsQueryHandler : IRequestHandler<GetAllMarketSubGroupsQuery, List<MarketSubGroupDTO>>
    {
        private readonly AppDbContext _context;

        /*
         * Constructor: GetAllMarketSubGroupsQueryHandler
         * Initializes the handler with the application's database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to query the database.
         */

        public GetAllMarketSubGroupsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Retrieves and returns a list of MarketSubGroupDTOs based on the provided query filters.
         *
         * Parameters:
         * - request: GetAllMarketSubGroupsQuery - Contains optional MarketId for filtering subgroups.
         * - cancellationToken: CancellationToken - Supports cancellation of the operation.
         *
         * Key Points:
         * 1. Fetches MarketSubGroups and filters by MarketId if provided.
         * 2. Projects the data into MarketSubGroupDTOs (only includes SubGroupId, SubGroupCode, SubGroupName, and MarketId).
         * 3. Sorts the subgroups first by numeric prefixes, then alphabetically.
         * 4. Returns the sorted list of subgroups.
         */
        public async Task<List<MarketSubGroupDTO>> Handle(GetAllMarketSubGroupsQuery request, CancellationToken cancellationToken)
        {

            var query = _context.MarketSubGroups.AsQueryable();

            if (request.MarketId.HasValue)
            {
                query = query.Where(sg => sg.MarketId == request.MarketId.Value);
            }

            var subGroups = await query
                .Select(sg => new MarketSubGroupDTO
                {
                    SubGroupId = sg.SubGroupId,
                    SubGroupCode = sg.SubGroupCode,
                    SubGroupName = sg.SubGroupName,
                    MarketId = sg.MarketId
                })
                .ToListAsync(cancellationToken);


            subGroups = subGroups
                .OrderBy(sg => HasNumericPrefix(sg.SubGroupCode) ? 0 : 1)
                .ThenBy(sg => GetNumericPrefix(sg.SubGroupCode))
                .ThenBy(sg => sg.SubGroupCode)
                .ToList();

            return subGroups;
        }

        /*
         * Helper Method: GetNumericPrefix
         * Extracts and returns the numeric prefix from a string (e.g., "12A" -> 12).
         *
         * Parameters:
         * - input: string - The input code to extract the numeric part.
         *
         * Returns:
         * - int? : The numeric prefix if found, or null if no digits are at the start.
         */
        private int? GetNumericPrefix(string input)
        {
            var numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(numericPart, out int result) ? result : (int?)null;
        }

        /*
         * Helper Method: HasNumericPrefix
         * Checks if a string starts with a numeric prefix.
         *
         * Parameters:
         * - input: string - The input code to check.
         *
         * Returns:
         * - bool: True if the code starts with a number, otherwise false.
         */
        private bool HasNumericPrefix(string input)
        {
            return char.IsDigit(input.FirstOrDefault());
        }
    }
}