using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MarketSubGroupDTO
    {
        public int SubGroupId { get; set; }
        public string SubGroupName { get; set; }
        public string SubGroupCode { get; set; }
        public int MarketId { get; set; }
        public string MarketCode { get; set; } 
    }
}
