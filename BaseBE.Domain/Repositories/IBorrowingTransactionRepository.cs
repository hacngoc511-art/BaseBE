using BaseBE.Domain.Entities;

namespace BaseBE.Domain.Repositories;

public interface IBorrowingTransactionRepository
{
    Task<BorrowingTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<BorrowingTransaction>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(BorrowingTransaction borrowingTransaction, CancellationToken cancellationToken = default);

    Task UpdateAsync(BorrowingTransaction borrowingTransaction, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}