csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CreateADotnetRepositoryController : ControllerBase
    {
        private readonly IRepositoryService _repositoryService;

        public CreateADotnetRepositoryController(IRepositoryService repositoryService)
        {
            _repositoryService = repositoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRepository(int id, CancellationToken cancellationToken)
        {
            var repository = await _repositoryService.GetRepositoryAsync(id, cancellationToken);
            if (repository == null)
            {
                return NotFound();
            }
            return Ok(repository);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRepository([FromBody] RepositoryDto repositoryDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRepository = await _repositoryService.CreateRepositoryAsync(repositoryDto, cancellationToken);
            return CreatedAtAction(nameof(GetRepository), new { id = createdRepository.Id }, createdRepository);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRepository(int id, [FromBody] RepositoryDto repositoryDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedRepository = await _repositoryService.UpdateRepositoryAsync(id, repositoryDto, cancellationToken);
            if (updatedRepository == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRepository(int id, CancellationToken cancellationToken)
        {
            var success = await _repositoryService.DeleteRepositoryAsync(id, cancellationToken);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}