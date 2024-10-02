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
    /**
     * @class GetAllMarketSubGroupsQueryHandler
     * Handles the retrieval of all MarketSubGroups from the database. 
     */
    public class GetAllMarketSubGroupsQueryHandler : IRequestHandler<GetAllMarketSubGroupsQuery, List<MarketSubGroupDTO>>
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
         * @return List<MarketSubGroupDTO> - A list of MarketSubGroups mapped to DTOs.
         */
        public async Task<List<MarketSubGroupDTO>> Handle(GetAllMarketSubGroupsQuery request, CancellationToken cancellationToken)
        {
            // Base query for MarketSubGroups
            var query = _context.MarketSubGroups
                .Include(subgroup => subgroup.Market)  // Include the related Market entity
                .AsQueryable();

            // Apply Market Code filter if provided
            if (!string.IsNullOrEmpty(request.MarketCode))
            {
                query = query.Where(subgroup => subgroup.Market.Code == request.MarketCode);
            }

            // Execute the query and fetch the data from the database
            var marketSubGroups = await query
                .ToListAsync(cancellationToken);

            // Filter the result in-memory to only include alphabetic SubGroupNames
            var filteredMarketSubGroups = marketSubGroups
                .Where(subgroup => IsAlphabetic(subgroup.SubGroupName)) // Ensure SubGroupName is alphabetic
                .OrderBy(subgroup => GetNumericPrefix(subgroup.SubGroupCode) == null) // Sort numeric codes first, alphabetic later
                .ThenBy(subgroup => GetNumericPrefix(subgroup.SubGroupCode)) // Sort by the numeric prefix if it exists
                .ThenBy(subgroup => subgroup.SubGroupCode) // Then sort by the full alphanumeric value
                .Select(subgroup => new MarketSubGroupDTO
                {
                    SubGroupId = subgroup.SubGroupId,
                    SubGroupName = subgroup.SubGroupName,
                    SubGroupCode = subgroup.SubGroupCode,
                    MarketId = subgroup.MarketId,
                    MarketCode = subgroup.Market.Code // Map the MarketCode from the Market entity
                })
                .ToList();

            return filteredMarketSubGroups;
        }

        // Helper method to check if SubGroupName contains only alphabetic characters
        private bool IsAlphabetic(string input)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

        // Helper function to extract the numeric prefix (returns null if there is no numeric part)
        private int? GetNumericPrefix(string input)
        {
            // Extracts the numeric part at the start of the string, if any
            var numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());

            if (int.TryParse(numericPart, out int result))
            {
                return result;
            }

            return null; // Return null if no numeric part found
        }
    }
}
