using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class MarketDetailsDto
    {
        public int MarketId { get; set; }
        public string MarketName { get; set; }
        public string MarketCode { get; set; }
        public string LongMarketCode { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public List<MarketSubGroupDto> MarketSubGroups { get; set; }
    }

    public class MarketSubGroupDto
    {
        public int SubGroupId { get; set; }
        public string SubGroupName { get; set; }
        public string SubGroupCode { get; set; }
    }

}
