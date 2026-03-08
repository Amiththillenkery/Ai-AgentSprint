using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using CreateADotnetRepositoryWithCleanArchitecture.Application.Interfaces;
using CreateADotnetRepositoryWithCleanArchitecture.Domain.Entities;
using CreateADotnetRepositoryWithCleanArchitecture.Application.Models;

namespace CreateADotnetRepositoryWithCleanArchitecture.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StructureTheCurrentRepositoryWithAddingSolutController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<ArticleModel> _validator;
        private readonly ILogger<StructureTheCurrentRepositoryWithAddingSolutController> _logger;

        public StructureTheCurrentRepositoryWithAddingSolutController(
            IArticleRepository articleRepository,
            IMapper mapper,
            IValidator<ArticleModel> validator,
            ILogger<StructureTheCurrentRepositoryWithAddingSolutController> logger)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _validator = validator;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(int id, CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found.", id);
                return NotFound();
            }
            return Ok(_mapper.Map<ArticleModel>(article));
        }

        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleModel articleModel, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(articleModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for article creation: {Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var article = _mapper.Map<Article>(articleModel);
            await _articleRepository.AddAsync(article, cancellationToken);
            return CreatedAtAction(nameof(GetArticle), new { id = article.Id }, _mapper.Map<ArticleModel>(article));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(int id, [FromBody] ArticleModel articleModel, CancellationToken cancellationToken)
        {
            if (id != articleModel.Id)
            {
                _logger.LogWarning("Article ID mismatch: {Id} != {ModelId}", id, articleModel.Id);
                return BadRequest("Article ID mismatch.");
            }

            var validationResult = await _validator.ValidateAsync(articleModel, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for article update: {Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found for update.", id);
                return NotFound();
            }

            _mapper.Map(articleModel, article);
            await _articleRepository.UpdateAsync(article, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id, CancellationToken cancellationToken)
        {
            var article = await _articleRepository.GetByIdAsync(id, cancellationToken);
            if (article == null)
            {
                _logger.LogWarning("Article with ID {Id} not found for deletion.", id);
                return NotFound();
            }

            await _articleRepository.DeleteAsync(article, cancellationToken);
            return NoContent();
        }
    }
}