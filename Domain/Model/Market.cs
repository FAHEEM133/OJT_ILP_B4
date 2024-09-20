using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class Market
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
        //create abstract class for Created on,Updated on.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for related MarketSubGroups
        public List<MarketSubGroup> MarketSubGroups { get; set; } = new List<MarketSubGroup>();
    }

}
