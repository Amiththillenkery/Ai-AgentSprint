using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using AutoMapper;
using Serilog;

namespace Application.Services
{
    public interface IStructureService
    {
        Task AddSolutionFoldersAsync();
    }

    public class StructureService : IStructureService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public StructureService(ApplicationDbContext context, IMapper mapper, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddSolutionFoldersAsync()
        {
            try
            {
                // Logic to add solution folders
                // This is a placeholder for the actual implementation
                _logger.Information("Adding solution folders...");

                // Simulate some async operation
                await Task.Delay(1000);

                _logger.Information("Solution folders added successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred while adding solution folders.");
                throw;
            }
        }
    }

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class ArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task AddArticleAsync(Article article)
        {
            if (article == null) throw new ArgumentNullException(nameof(article));

            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }
    }

    public class ArticleValidator : AbstractValidator<Article>
    {
        public ArticleValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IStructureService, StructureService>();
            services.AddScoped<ArticleRepository>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddValidatorsFromAssemblyContaining<ArticleValidator>();
            return services;
        }
    }
}