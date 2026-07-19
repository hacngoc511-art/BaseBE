using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Repositories;

public class BorrowingTransactionRepository : IBorrowingTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public BorrowingTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BorrowingTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.BorrowingTransactions
            .Include(x => x.Member)
            .Include(x => x.Book)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<BorrowingTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BorrowingTransactions
            .Include(x => x.Member)
            .Include(x => x.Book)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BorrowingTransaction borrowingTransaction, CancellationToken cancellationToken = default)
    {
        await _context.BorrowingTransactions.AddAsync(borrowingTransaction, cancellationToken);
    }

    public Task UpdateAsync(BorrowingTransaction borrowingTransaction, CancellationToken cancellationToken = default)
    {
        _context.BorrowingTransactions.Update(borrowingTransaction);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var borrowingTransaction = await GetByIdAsync(id, cancellationToken);

        if (borrowingTransaction != null)
        {
            _context.BorrowingTransactions.Remove(borrowingTransaction);
        }
    }
}