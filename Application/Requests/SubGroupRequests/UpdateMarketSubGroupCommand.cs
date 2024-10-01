using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class UpdateMarketSubGroupCommand : IRequest<MarketSubGroupDTO>
    {
        public int SubGroupId { get; set; }

        [Required]
        [MaxLength(150)]
        public string SubGroupName { get; set; }

        [Required]
        [MaxLength(1)]
        public string SubGroupCode { get; set; }

        [Required]
        public int MarketId { get; set; }
    }
}
