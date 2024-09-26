using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.RegionRequests
{
    public class GetAllRegionsQuery : IRequest<List<Region>>
    {

    }
}
