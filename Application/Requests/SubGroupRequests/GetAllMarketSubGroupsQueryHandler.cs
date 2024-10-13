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
