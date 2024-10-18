using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Enums.Domain.Enums;
namespace Domain.Model
{
    public class Market : AuditableEntity
    {
        public int Id { get; set; }  

        [Required]
        [MaxLength(150)]
        public string Name { get; set; }  

        [Required]
        [MaxLength(2)]
        public string Code { get; set; }  

        [Required]
        [MaxLength(50)]
        public string LongMarketCode { get; set; }  

        [Required]
        public Region Region { get; set; } 

        [Required]
        public SubRegion SubRegion { get; set; }  

        
        public List<MarketSubGroup> MarketSubGroups { get; set; } = new List<MarketSubGroup>();
    }
}
