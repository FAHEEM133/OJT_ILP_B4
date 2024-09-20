using Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IMarketRepository
    {
        // Get all markets
        Task<IEnumerable<Market>> GetAllMarketsAsync();

        // Get a market by its ID
        Task<Market> GetMarketByIdAsync(int id);

        // Get a market by its name (used to check for uniqueness)
        Task<Market> GetMarketByNameAsync(string marketName);

        // Add a new market
        Task<Market> AddMarketAsync(Market market);

        // Update an existing market
        Task<bool> UpdateMarketAsync(Market market);

        // Delete a market by its ID
        Task<bool> DeleteMarketAsync(int id);
    }
}
