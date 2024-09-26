using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class DeleteMarketSubGroupCommand : IRequest<bool>
    {
        public int SubGroupId { get; set; }

        public DeleteMarketSubGroupCommand(int subGroupId)
        {
            SubGroupId = subGroupId;
        }
    }
}
