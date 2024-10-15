using Application.Validations;
using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{
    public class UpdateMarketCommandHandler : IRequestHandler<UpdateMarketCommand, object>
    {
        private readonly AppDbContext _context;

        public UpdateMarketCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(UpdateMarketCommand request, CancellationToken cancellationToken)
        {
            var existingMarket = await _context.Markets
                .Include(m => m.MarketSubGroups) 
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (existingMarket == null)
            {
                throw new ValidationException($"Market with ID {request.Id} not found.");
            }

            
            if (!RegionSubRegionValidation.IsValidSubRegionForRegion(request.Region, request.SubRegion))
            {
                throw new ValidationException($"SubRegion {request.SubRegion} is not valid for the Region {request.Region}");
            }

            
            if (request.Name != null && existingMarket.Name != request.Name)
            {
                var existingMarketByName = await _context.Markets
                    .FirstOrDefaultAsync(m => m.Name == request.Name && m.Id != request.Id, cancellationToken);

                if (existingMarketByName != null)
                {
                    throw new ValidationException("A market with this name already exists.");
                }
            }

            if (request.Code != null && existingMarket.Code != request.Code)
            {
                var existingMarketByCode = await _context.Markets
                    .FirstOrDefaultAsync(m => m.Code == request.Code && m.Id != request.Id, cancellationToken);

                if (existingMarketByCode != null)
                {
                    throw new ValidationException("A market with this code already exists.");
                }
            }

            existingMarket.Id = request.Id;
            existingMarket.Name = request.Name ?? existingMarket.Name;
            existingMarket.Code = request.Code ?? existingMarket.Code;
            existingMarket.LongMarketCode = request.LongMarketCode ?? existingMarket.LongMarketCode;
            existingMarket.Region = request.Region;
            existingMarket.SubRegion = request.SubRegion;

            
            var existingSubGroups = existingMarket.MarketSubGroups.ToList();

           
            var subGroupsToRemove = existingSubGroups
                .Where(sg => !request.MarketSubGroups.Any(reqSg => reqSg.SubGroupId == sg.SubGroupId))
                .ToList();

            foreach (var subGroupToRemove in subGroupsToRemove)
            {
                _context.MarketSubGroups.Remove(subGroupToRemove);
            }

           
            foreach (var requestSubGroup in request.MarketSubGroups)
            {
                var existingSubGroup = existingSubGroups
                    .FirstOrDefault(sg => sg.SubGroupId == requestSubGroup.SubGroupId);

                if (existingSubGroup != null)
                {
                    
                    existingSubGroup.SubGroupName = requestSubGroup.SubGroupName;
                    existingSubGroup.SubGroupCode = requestSubGroup.SubGroupCode;
                    existingSubGroup.MarketId = existingMarket.Id;
                }
                else
                {
                 
                    var newSubGroup = new MarketSubGroup
                    {
                        SubGroupName = requestSubGroup.SubGroupName, 
                        SubGroupCode = requestSubGroup.SubGroupCode, 
                        MarketId = existingMarket.Id 
                    };
                    _context.MarketSubGroups.Add(newSubGroup);
                }
            }

            
            await _context.SaveChangesAsync(cancellationToken);

            
            var updatedMarket = await _context.Markets
                .Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

           
            return new
            {
                id = updatedMarket.Id,
                name = updatedMarket.Name,
                code = updatedMarket.Code,
                longMarketCode = updatedMarket.LongMarketCode,
                region = (int)updatedMarket.Region,
                subRegion = (int)updatedMarket.SubRegion,
                marketSubGroups = updatedMarket.MarketSubGroups.Select(sg => new
                {
                    subGroupId = sg.SubGroupId,
                    subGroupName = sg.SubGroupName,
                    subGroupCode = sg.SubGroupCode,
                    marketId = sg.MarketId,
                    marketCode = updatedMarket.Code
                }).ToList()
            };
        }
    }
}