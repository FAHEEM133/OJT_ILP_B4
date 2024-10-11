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
         * Initializes the handler with the database context.
         * 
         * Parameters:
         * - context: AppDbContext - The application's database context used to interact with the database.
         */
        public GetAllMarketSubGroupsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /*
         * Method: Handle
         * Retrieves all market subgroup entries based on the market code filter (if provided) and filters alphabetic subgroups.
         * 
         * Parameters:
         * - request: GetAllMarketSubGroupsQuery - The query object used for fetching all market subgroups.
         * - cancellationToken: CancellationToken - Token for handling operation cancellation.
         * 
         * Returns:
         * - Task<List<MarketSubGroupDTO>>: Asynchronously returns a list of filtered MarketSubGroupDTO entities.
         */

        public async Task<List<MarketSubGroupDTO>> Handle(GetAllMarketSubGroupsQuery request, CancellationToken cancellationToken)
        {
            /*
             * LLD Steps:
             * 1. Access the MarketSubGroups DbSet from the AppDbContext and include the related Market entity.
             * 2. If a MarketCode is provided in the request, filter the MarketSubGroups based on it.
             * 3. Fetch the filtered market subgroups from the database asynchronously using `ToListAsync` with the cancellationToken.
             * 4. Filter market subgroups to only include those with alphabetic SubGroupNames.
             * 5. Order the results by the numeric prefix (if present) and then by SubGroupCode.
             * 6. Map the filtered and ordered subgroups to the MarketSubGroupDTO format and return the list.
             */
            var query = _context.MarketSubGroups
                .Include(subgroup => subgroup.Market)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.MarketCode))
            {
                query = query.Where(subgroup => subgroup.Market.Code == request.MarketCode);
            }

            var marketSubGroups = await query.ToListAsync(cancellationToken);

            var filteredMarketSubGroups = marketSubGroups
                .Where(subgroup => IsAlphabetic(subgroup.SubGroupName))
                .OrderBy(subgroup => GetNumericPrefix(subgroup.SubGroupCode) == null)
                .ThenBy(subgroup => GetNumericPrefix(subgroup.SubGroupCode))
                .ThenBy(subgroup => subgroup.SubGroupCode)
                .Select(subgroup => new MarketSubGroupDTO
                {
                    SubGroupId = subgroup.SubGroupId,
                    SubGroupName = subgroup.SubGroupName,
                    SubGroupCode = subgroup.SubGroupCode,
                    MarketId = subgroup.MarketId,
                    MarketCode = subgroup.Market.Code
                })
                .ToList();

            return filteredMarketSubGroups;
        }

        /*
         * Method: IsAlphabetic
         * Validates if the input string contains only alphabetic characters.
         * 
         * Parameters:
         * - input: string - The input string to validate.
         * 
         * Returns:
         * - bool: True if the string contains only alphabetic characters, otherwise false.
         */
        private bool IsAlphabetic(string input)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

        /*
         * Method: GetNumericPrefix
         * Extracts the numeric prefix from a given string, if present.
         * 
         * Parameters:
         * - input: string - The input string to extract the numeric prefix from.
         * 
         * Returns:
         * - int?: The numeric prefix as an integer if found, otherwise null.
         */
        private int? GetNumericPrefix(string input)
        {
            var numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());

            if (int.TryParse(numericPart, out int result))
            {
                return result;
            }

            return null;
        }
    }
}
