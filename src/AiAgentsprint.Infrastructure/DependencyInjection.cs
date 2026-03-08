using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImplementArticleEntity.Infrastructure.Persistence;
using ImplementArticleEntity.Infrastructure.Repositories;
using ImplementArticleEntity.Domain.Repositories;

namespace ImplementArticleEntity.Infrastructure
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

namespace ImplementArticleEntity.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });
        }
    }
}

namespace ImplementArticleEntity.Domain.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
    }
}

namespace ImplementArticleEntity.Domain.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> GetByIdAsync(int id);
        Task<IEnumerable<Article>> GetAllAsync();
        Task AddAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(int id);
    }
}

namespace ImplementArticleEntity.Infrastructure.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Article> GetByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task AddAsync(Article article)
        {
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
            }
        }
    }
}