using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureTheCurrentRepositoryWithAddingSolut.Infrastructure.Repositories;

namespace StructureTheCurrentRepositoryWithAddingSolut.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IArticleRepository, ArticleRepository>();

            return services;
        }
    }
}

namespace StructureTheCurrentRepositoryWithAddingSolut.Infrastructure.Repositories
{
    public interface IArticleRepository
    {
        // Define repository methods
    }

    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implement repository methods
    }
}

namespace StructureTheCurrentRepositoryWithAddingSolut.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSets for your entities
    }
}