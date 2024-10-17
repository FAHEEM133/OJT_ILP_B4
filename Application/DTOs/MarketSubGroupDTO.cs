using System.Text.Json.Serialization;

namespace Application.DTOs;

public class MarketSubGroupDTO
{
    [JsonIgnore]
    public int SubGroupId { get; set; }
    public string SubGroupName { get; set; }
    public string SubGroupCode { get; set; }

    [JsonIgnore]
    public int MarketId { get; set; }

}
