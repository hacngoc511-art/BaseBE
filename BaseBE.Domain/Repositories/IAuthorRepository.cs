using BaseBE.Domain.Entities;

namespace BaseBE.Domain.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
    Task UpdateAsync(Author author, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}