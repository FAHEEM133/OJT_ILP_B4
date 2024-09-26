using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class ConfigurationServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
<<<<<<< HEAD
            // Register DbContext with PostgreSQL connection string from configuration
=======
            
>>>>>>> 5fa7efa15c9090ae4e0c64691b39b5ab74ea656a
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

<<<<<<< HEAD
            // Register Repositories

=======
            
>>>>>>> 5fa7efa15c9090ae4e0c64691b39b5ab74ea656a

            return services;
        }


    }
}
