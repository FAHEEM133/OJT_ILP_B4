using Application.Validations;
using Domain.Enums.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
  public class GetSubRegionsByRegionQueryHandler : IRequestHandler<GetSubRegionsByRegionQuery, List<SubRegion>>
    {
        public Task<List<SubRegion>> Handle(GetSubRegionsByRegionQuery request, CancellationToken cancellationToken)
        {
            // Get all subregions based on the region using the validation logic
            var subRegions = RegionSubRegionValidation.GetSubRegionsForRegion(request.Region);
            return Task.FromResult(subRegions);
        }
    }
}
