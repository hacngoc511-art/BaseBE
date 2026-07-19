using BaseBE.Domain.Entities;

namespace BaseBE.Domain.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<Member>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Member member, CancellationToken cancellationToken = default);

    Task UpdateAsync(Member member, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}