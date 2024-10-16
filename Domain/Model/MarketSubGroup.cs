using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class MarketSubGroup:AuditableEntity
    {
        public int SubGroupId { get; set; }

        [Required]
        [MaxLength(150)]
        public string SubGroupName { get; set; }

        [Required]
        [MaxLength(1)]
        public string SubGroupCode { get; set; }

        public int MarketId { get; set; }

        [JsonIgnore]
        public Market Market { get; set; }

      
    }

}
