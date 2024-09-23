using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetAllRegionsQueryHandler : IRequestHandler<GetAllRegionsQuery, List<Region>>
    {
        public Task<List<Region>> Handle(GetAllRegionsQuery request, CancellationToken cancellationToken)
        {
            var regions = Enum.GetValues(typeof(Region)).Cast<Region>().ToList();
            return Task.FromResult(regions);
        }
    }
}
