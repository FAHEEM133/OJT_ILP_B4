using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public class MarketDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string LongMarketCode { get; set; }
    public string Region { get; set; }
    public string SubRegion { get; set; }
    public List<MarketSubGroupDTO> MarketSubGroups { get; set; }
}
