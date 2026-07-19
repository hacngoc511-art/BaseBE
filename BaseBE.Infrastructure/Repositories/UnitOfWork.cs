using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;

namespace BaseBE.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Students = new StudentRepository(_context);
        Authors = new AuthorRepository(_context);
        Members = new MemberRepository(_context);
        BorrowingTransactions = new BorrowingTransactionRepository(_context);
    }

    public IStudentRepository Students { get; }

    public IAuthorRepository Authors { get; }

    public IMemberRepository Members { get; }

    public IBorrowingTransactionRepository BorrowingTransactions { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}