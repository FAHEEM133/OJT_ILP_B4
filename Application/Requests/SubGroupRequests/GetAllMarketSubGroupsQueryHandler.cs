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

        private bool IsAlphabetic(string input)
        {
            var regex = new Regex("^[a-zA-Z]+$");
            return regex.IsMatch(input);
        }

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
