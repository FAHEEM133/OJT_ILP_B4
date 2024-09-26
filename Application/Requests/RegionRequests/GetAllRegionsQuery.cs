using MediatR;
using System.Collections.Generic;

namespace Application.Requests.RegionRequests
{
    public class GetAllRegionsQuery : IRequest<List<KeyValuePair<int, string>>>
    {
    }
}
