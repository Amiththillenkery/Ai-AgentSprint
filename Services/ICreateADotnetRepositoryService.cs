csharp
namespace Services
{
    public interface ICreateADotnetRepositoryService
    {
        Task CreateAsync<T>(T entity, CancellationToken cancellationToken);
        Task<T> ReadAsync<T>(Guid id, CancellationToken cancellationToken);
        Task UpdateAsync<T>(T entity, CancellationToken cancellationToken);
        Task DeleteAsync<T>(Guid id, CancellationToken cancellationToken);
    }
}