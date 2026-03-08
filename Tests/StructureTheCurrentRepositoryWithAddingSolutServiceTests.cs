using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;

namespace Tests
{
    public class StructureTheCurrentRepositoryWithAddingSolutServiceTests
    {
        private readonly Mock<DbSet<Article>> _mockSet;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly ArticleRepository _repository;

        public StructureTheCurrentRepositoryWithAddingSolutServiceTests()
        {
            _mockSet = new Mock<DbSet<Article>>();
            _mockContext = new Mock<ApplicationDbContext>();
            _mockContext.Setup(m => m.Articles).Returns(_mockSet.Object);
            _repository = new ArticleRepository(_mockContext.Object);
        }

        [Fact]
        public async Task AddArticle_ShouldAddArticleToDatabase()
        {
            var article = new Article { Id = 1, Title = "Test Article" };

            await _repository.AddArticleAsync(article);

            _mockSet.Verify(m => m.AddAsync(It.IsAny<Article>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetArticles_ShouldReturnAllArticles()
        {
            var data = new List<Article>
            {
                new Article { Id = 1, Title = "Test Article 1" },
                new Article { Id = 2, Title = "Test Article 2" }
            }.AsQueryable();

            _mockSet.As<IQueryable<Article>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var articles = await _repository.GetArticlesAsync();

            articles.Should().HaveCount(2);
            articles.Should().Contain(a => a.Title == "Test Article 1");
            articles.Should().Contain(a => a.Title == "Test Article 2");
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnCorrectArticle()
        {
            var data = new List<Article>
            {
                new Article { Id = 1, Title = "Test Article 1" },
                new Article { Id = 2, Title = "Test Article 2" }
            }.AsQueryable();

            _mockSet.As<IQueryable<Article>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var article = await _repository.GetArticleByIdAsync(1);

            article.Should().NotBeNull();
            article.Title.Should().Be("Test Article 1");
        }

        [Fact]
        public async Task GetArticleById_ShouldReturnNull_WhenArticleNotFound()
        {
            var data = new List<Article>().AsQueryable();

            _mockSet.As<IQueryable<Article>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Article>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var article = await _repository.GetArticleByIdAsync(1);

            article.Should().BeNull();
        }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
    }

    public class ArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddArticleAsync(Article article)
        {
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            return await _context.Articles.ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }
    }
}