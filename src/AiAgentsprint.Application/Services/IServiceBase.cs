using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ImplementArticleEntitiy.Application.Services
{
    public interface IServiceBase<TDto, TCreateDto, TUpdateDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TDto> CreateAsync(TCreateDto createDto, CancellationToken cancellationToken = default);
        Task<TDto> UpdateAsync(int id, TUpdateDto updateDto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}