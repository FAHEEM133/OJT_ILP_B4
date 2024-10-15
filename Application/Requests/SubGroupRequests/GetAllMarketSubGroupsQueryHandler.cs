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
    /// <summary>
    /// Handles the retrieval of all market subgroups, with optional filtering by MarketId.
    /// This class queries the database to get the required data, sorts it, and returns the subgroups
    /// in a structured format (MarketSubGroupDTO).
    /// </summary>
    public class GetAllMarketSubGroupsQueryHandler : IRequestHandler<GetAllMarketSubGroupsQuery, List<MarketSubGroupDTO>>
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllMarketSubGroupsQueryHandler"/> class.
        /// </summary>
        /// <param name="context">The application's database context used to query the MarketSubGroups table.</param>
        public GetAllMarketSubGroupsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Handles the query to retrieve all market subgroups, optionally filtering by MarketId.
        /// </summary>
        /// <param name="request">The request containing the MarketId (optional) to filter the subgroups.</param>
        /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
        /// <returns>A list of <see cref="MarketSubGroupDTO"/> representing the subgroups, sorted by numeric prefix and then alphabetically.</returns>
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

        /// <summary>
        /// Extracts the numeric prefix from a given string (e.g., "12A" returns 12).
        /// </summary>
        /// <param name="input">The input string to extract the numeric part from.</param>
        /// <returns>The numeric prefix as an integer, or null if no numeric prefix exists.</returns>
        private int? GetNumericPrefix(string input)
        {
            var numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(numericPart, out int result) ? result : (int?)null;
        }

        /// <summary>
        /// Determines if a string starts with a numeric prefix.
        /// </summary>
        /// <param name="input">The input string to check for a numeric prefix.</param>
        /// <returns>True if the string starts with a number, otherwise false.</returns>
        private bool HasNumericPrefix(string input)
        {
            return char.IsDigit(input.FirstOrDefault());
        }
    }
}
