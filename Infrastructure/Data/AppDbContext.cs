using Domain.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Market> Markets { get; set; }
        public DbSet<MarketSubGroup> MarketSubGroups { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the unique constraints for Market entity
            modelBuilder.Entity<Market>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<Market>()
                .HasIndex(m => m.Code)
                .IsUnique();

            // Define the composite unique constraint for MarketSubGroup within a Market
            modelBuilder.Entity<MarketSubGroup>()
                .HasIndex(sg => new { sg.MarketId, sg.SubGroupName })
                .IsUnique();

            modelBuilder.Entity<MarketSubGroup>()
                .HasIndex(sg => new { sg.MarketId, sg.SubGroupCode })
                .IsUnique();
           
            modelBuilder.Entity<MarketSubGroup>()
                 .HasKey(sg => sg.SubGroupId);

            modelBuilder.Entity<Market>()
                .HasKey(m => m.Id);

            // Configure the one-to-many relationship between Market and MarketSubGroup
            modelBuilder.Entity<MarketSubGroup>()
                .HasOne(sg => sg.Market)
                .WithMany(m => m.MarketSubGroups)
                .HasForeignKey(sg => sg.MarketId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            // Cascade delete when Market is deleted
        }
    }
}
