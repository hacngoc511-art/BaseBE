namespace BaseBE.Domain.Repositories;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }

    IAuthorRepository Authors { get; }

    IMemberRepository Members { get; }

    IBorrowingTransactionRepository BorrowingTransactions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}