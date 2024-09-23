using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Enums.Domain.Enums;

namespace Domain.Model
{
    public class Market : AuditableEntity
    {
        public int Id { get; set; }  // Primary Key

        [Required]
        [MaxLength(150)]
        public string MarketName { get; set; }  // Unique

        [Required]
        [MaxLength(2)]
        public string MarketCode { get; set; }  // Unique, 2 letters

        [Required]
        [MaxLength(50)]
        public string LongMarketCode { get; set; }  // Optional field for long code

        [Required]
        public Region Region { get; set; }  // Enum for Region

        [Required]
        public SubRegion SubRegion { get; set; }  // Enum for SubRegion

        // Navigation property for related MarketSubGroups
        public List<MarketSubGroup> MarketSubGroups { get; set; } = new List<MarketSubGroup>();
    }
}
