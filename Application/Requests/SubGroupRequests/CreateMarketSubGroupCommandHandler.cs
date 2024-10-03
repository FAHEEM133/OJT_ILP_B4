using Domain.Model;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Requests.SubGroupRequests
{
    /**
     * @class CreateMarketSubGroupCommandHandler
     * Handles the creation of a new MarketSubGroup by validating the existence of the Market, 
     * ensuring the uniqueness of the SubGroup, and saving it to the database.
     */
    public class CreateMarketSubGroupCommandHandler : IRequestHandler<CreateMarketSubGroupCommand, int>
    {
        private readonly AppDbContext _context;
       /**
        * Constructor
        * @param context AppDbContext - Injected database context for interacting with Market and MarketSubGroup entities.
        */

        public CreateMarketSubGroupCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        /**
         * Handles the command to create a MarketSubGroup.
         * @param request CreateMarketSubGroupCommand - Contains the SubGroup data.
         * @param cancellationToken CancellationToken - Allows cancellation of the task.
         * 
         * Validates Market existence, ensures SubGroup name/code uniqueness, 
         * creates the SubGroup, and saves it to the database.
         * 
         * @return int - The ID of the newly created MarketSubGroup.
         * @throws Exception - If Market is not found or if SubGroup name/code is not unique.
         */
        public async Task<int> Handle(CreateMarketSubGroupCommand request, CancellationToken cancellationToken)
        {
            // Step 1: Validate if the Market exists using MarketCode
            var market = await _context.Markets
                .FirstOrDefaultAsync(m => m.Code == request.MarketCode, cancellationToken);

            if (market == null)
            {
                throw new Exception("Market not found");
            }

            // Step 2: Use the found MarketId instead of receiving it from the request
            var marketId = market.Id;

            // Ensure SubGroupName contains only alphabetic characters (case insensitive)
            var nameRegex = new Regex("^[a-zA-Z]+$");
            if (!nameRegex.IsMatch(request.SubGroupName))
            {
                throw new Exception("SubGroupName can only contain alphabetic characters.");
            }

            // Convert SubGroupName and SubGroupCode to lowercase for case-insensitive comparison
            var lowerCaseSubGroupName = request.SubGroupName.ToLower();
            var lowerCaseSubGroupCode = request.SubGroupCode.ToLower();

            // Step 3: Check if SubGroupName or SubGroupCode already exists in the given market using the found MarketId
            var isSubGroupNameExists = await _context.MarketSubGroups.AnyAsync(
                sg => sg.MarketId == marketId &&
                      sg.SubGroupName.ToLower() == lowerCaseSubGroupName,
                cancellationToken
            );

            var isSubGroupCodeExists = await _context.MarketSubGroups.AnyAsync(
                sg => sg.MarketId == marketId &&
                      sg.SubGroupCode.ToLower() == lowerCaseSubGroupCode,
                cancellationToken
            );

            // If both SubGroupName and SubGroupCode are repeating together
            var isBothExists = await _context.MarketSubGroups.AnyAsync(
                sg => sg.MarketId == marketId &&
                      sg.SubGroupName.ToLower() == lowerCaseSubGroupName &&
                      sg.SubGroupCode == request.SubGroupCode,
                cancellationToken
            );

            // Separate error messages for each validation
            if (isBothExists)
            {
                throw new Exception("A subgroup with the same name and code already exists within this market.");
            }

            if (isSubGroupNameExists)
            {
                throw new Exception("A subgroup with the same name already exists within this market.");
            }

            if (isSubGroupCodeExists)
            {
                throw new Exception("A subgroup with the same code already exists within this market.");
            }

            // Step 4: Alphanumeric validation for SubGroupCode (already added)
            var codeRegex = new Regex("^[a-zA-Z0-9]*$");
            if (!codeRegex.IsMatch(request.SubGroupCode))
            {
                throw new Exception("Invalid format for subgroup code.");
            }

            // Step 5: Proceed to create the new MarketSubGroup using the found MarketId
            var marketSubGroup = new MarketSubGroup
            {
                SubGroupName = request.SubGroupName,
                SubGroupCode = request.SubGroupCode,
                MarketId = marketId,  // Use the found MarketId
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MarketSubGroups.Add(marketSubGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return marketSubGroup.SubGroupId;
        }

    }
}
