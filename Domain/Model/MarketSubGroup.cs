using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class MarketSubGroup
    {
        public int SubGroupId { get; set; }  // Primary Key

        [Required]
        [MaxLength(150)]
        public string SubGroupName { get; set; }  // Unique within the same market

        [Required]
        [MaxLength(1)]
        public string SubGroupCode { get; set; }  // Unique within the same market

        // Foreign Key
        public int MarketId { get; set; }

        // Navigation property to Market
        public Market Market { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
