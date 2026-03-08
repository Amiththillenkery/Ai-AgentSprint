using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImplementArticleEntitiy.Application.Services
{
    public interface IServiceBase<TDto, TCreateDto, TUpdateDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken);
        Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}