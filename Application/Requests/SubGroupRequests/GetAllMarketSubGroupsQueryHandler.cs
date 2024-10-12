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

        public GetAllMarketSubGroupsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MarketSubGroupDTO>> Handle(GetAllMarketSubGroupsQuery request, CancellationToken cancellationToken)
        {
            // Query all subgroups, optionally filter by marketId if provided
            var query = _context.MarketSubGroups.AsQueryable();

            if (request.MarketId.HasValue)
            {
                query = query.Where(sg => sg.MarketId == request.MarketId.Value);
            }

            // Fetch data first, then apply filtering and sorting in memory
            var subGroups = await query
                .Select(sg => new MarketSubGroupDTO
                {
                    SubGroupId = sg.SubGroupId,
                    SubGroupCode = sg.SubGroupCode,
                    SubGroupName = sg.SubGroupName,
                    MarketId = sg.MarketId
                })
                .ToListAsync(cancellationToken);

            // Apply filtering and sorting in memory
            subGroups = subGroups
                .OrderBy(sg => HasNumericPrefix(sg.SubGroupCode) ? 0 : 1) // Give preference to codes with numbers
                .ThenBy(sg => GetNumericPrefix(sg.SubGroupCode)) // Sort by numeric prefix
                .ThenBy(sg => sg.SubGroupCode) // Then by the full code
                .ToList();

            return subGroups;
        }

        private int? GetNumericPrefix(string input)
        {
            var numericPart = new string(input.TakeWhile(char.IsDigit).ToArray());
            return int.TryParse(numericPart, out int result) ? result : (int?)null;
        }

        private bool HasNumericPrefix(string input)
        {
            return char.IsDigit(input.FirstOrDefault());
        }
    }
}
