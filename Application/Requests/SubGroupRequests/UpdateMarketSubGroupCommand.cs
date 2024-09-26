﻿using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    public class UpdateMarketSubGroupCommand : IRequest<int>
    {
        public int SubGroupId { get; set; } 

        public string SubGroupName { get; set; }  

        public string SubGroupCode { get; set; }  

        public int MarketId { get; set; } 
    }
}