using Domain.Model;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MarketRepository : IMarketRepository
    {
        private readonly AppDbContext _context;

        public MarketRepository(AppDbContext context)
        {
            _context = context;
        }

        // Fetch all markets
        public async Task<IEnumerable<Market>> GetAllMarketsAsync()
        {
            return await _context.Markets.Include(m => m.MarketSubGroups).ToListAsync();
        }

        // Fetch a market by its ID
        public async Task<Market> GetMarketByIdAsync(int id)
        {
            return await _context.Markets.Include(m => m.MarketSubGroups)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Fetch a market by its name
        public async Task<Market> GetMarketByNameAsync(string marketName)
        {
            return await _context.Markets.FirstOrDefaultAsync(m => m.MarketName == marketName);
        }

        // Add a new market
        public async Task<Market> AddMarketAsync(Market market)
        {
            _context.Markets.Add(market);
            await _context.SaveChangesAsync();
            return market;
        }

        // Update an existing market
        public async Task<bool> UpdateMarketAsync(Market market)
        {
            var existingMarket = await _context.Markets.FindAsync(market.Id);
            if (existingMarket == null)
            {
                return false;
            }

            existingMarket.MarketName = market.MarketName;
            existingMarket.MarketCode = market.MarketCode;
            existingMarket.LongMarketCode = market.LongMarketCode;
           
            existingMarket.UpdatedAt = market.UpdatedAt;

            _context.Markets.Update(existingMarket);
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete a market by its ID
        public async Task<bool> DeleteMarketAsync(int id)
        {
            var market = await _context.Markets.FindAsync(id);
            if (market == null)
            {
                return false;
            }

            _context.Markets.Remove(market);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
