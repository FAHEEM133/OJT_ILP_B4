namespace Application.DTOs
{
    public class MarketSubGroupDTO
    {
        
        public int? SubGroupId { get; set; }
        public string SubGroupName { get; set; }
        public string SubGroupCode { get; set; }
        public int MarketId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsEdited { get; set; } = false;
    }
}
