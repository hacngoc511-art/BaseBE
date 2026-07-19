using BaseBE.Application.DTOs;
using BaseBE.Application.Queries;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class BorrowingTransactionQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BorrowingTransactionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BorrowingTransactionDto>> HandleAsync(
        GetBorrowingTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var borrowings = await _unitOfWork.BorrowingTransactions.GetAllAsync(cancellationToken);

        return borrowings.Select(x => new BorrowingTransactionDto(
            x.Id,
            x.MemberId,
            x.BookId,
            x.BorrowDate,
            x.ReturnDate,
            x.IsReturned));
    }

    public async Task<BorrowingTransactionDto?> HandleAsync(
        GetBorrowingTransactionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var borrowing = await _unitOfWork.BorrowingTransactions.GetByIdAsync(query.Id, cancellationToken);

        if (borrowing == null)
            return null;

        return new BorrowingTransactionDto(
            borrowing.Id,
            borrowing.MemberId,
            borrowing.BookId,
            borrowing.BorrowDate,
            borrowing.ReturnDate,
            borrowing.IsReturned);
    }
}