using Domain.Enums;
using MediatR;
using System.Collections.Generic;

namespace Application.Requests.RegionRequests
{
    public class GetSubRegionsByRegionQuery : IRequest<List<KeyValuePair<int, string>>>
    {
        public Region Region { get; set; }
    }
}
