csharp
namespace Services
{
    public interface ICreateADotnetRepositoryService
    {
        Task CreateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken);
        Task<TEntity> ReadAsync<TEntity>(Guid id, CancellationToken cancellationToken);
        Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken);
        Task DeleteAsync<TEntity>(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> ListAsync<TEntity>(CancellationToken cancellationToken);
    }
}