using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class CreateMarketSubGroupCommand : IRequest<int>
    {
        [Required]
        [MaxLength(150)]
        public string SubGroupName { get; set; }

        [Required]
        [MaxLength(1)]
        public string SubGroupCode { get; set; }


        [Required]
        [MaxLength(2)]
        public string MarketCode { get; set; }
    }
}
