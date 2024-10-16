﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Requests.MarketRequests
{ 
    /// <summary>
    /// Query to check if a market name exists in the database.
    /// </summary>
    public class CheckMarketNameExistsQuery : IRequest<bool>
    {
        /// <summary>
        /// Gets or sets the market name that needs to be checked for existence.
        /// </summary>
        public string Name { get; set; }
    }
}
