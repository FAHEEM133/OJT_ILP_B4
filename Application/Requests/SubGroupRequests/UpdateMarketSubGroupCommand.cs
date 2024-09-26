using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class UpdateMarketSubGroupCommand : IRequest<int>
    {
        public int SubGroupId { get; set; }  // Primary Key

        public string SubGroupName { get; set; }  // Unique within the same market

        public string SubGroupCode { get; set; }  // Unique within the same market

        public int MarketId { get; set; }  // Foreign Key to Market
    }
}
