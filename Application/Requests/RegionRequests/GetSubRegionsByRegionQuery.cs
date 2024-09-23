using Domain.Enums;
using Domain.Enums.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetSubRegionsByRegionQuery : IRequest<List<SubRegion>>
    {
        public Region Region { get; set; }
    }
}
