using BaseBE.Application.Commands;
using BaseBE.Application.DTOs;
using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class BorrowingTransactionCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public BorrowingTransactionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BorrowingTransactionDto> CreateAsync(CreateBorrowingTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var borrowing = new BorrowingTransaction
        {
            MemberId = command.MemberId,
            BookId = command.BookId,
            BorrowDate = command.BorrowDate,
            ReturnDate = command.ReturnDate,
            IsReturned = command.IsReturned
        };

        await _unitOfWork.BorrowingTransactions.AddAsync(borrowing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BorrowingTransactionDto(
            borrowing.Id,
            borrowing.MemberId,
            borrowing.BookId,
            borrowing.BorrowDate,
            borrowing.ReturnDate,
            borrowing.IsReturned);
    }

    public async Task<BorrowingTransactionDto?> UpdateAsync(UpdateBorrowingTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var borrowing = await _unitOfWork.BorrowingTransactions.GetByIdAsync(command.Id, cancellationToken);

        if (borrowing == null)
            return null;

        borrowing.MemberId = command.MemberId;
        borrowing.BookId = command.BookId;
        borrowing.BorrowDate = command.BorrowDate;
        borrowing.ReturnDate = command.ReturnDate;
        borrowing.IsReturned = command.IsReturned;

        await _unitOfWork.BorrowingTransactions.UpdateAsync(borrowing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new BorrowingTransactionDto(
            borrowing.Id,
            borrowing.MemberId,
            borrowing.BookId,
            borrowing.BorrowDate,
            borrowing.ReturnDate,
            borrowing.IsReturned);
    }

    public async Task<bool> DeleteAsync(DeleteBorrowingTransactionCommand command, CancellationToken cancellationToken = default)
    {
        var borrowing = await _unitOfWork.BorrowingTransactions.GetByIdAsync(command.Id, cancellationToken);

        if (borrowing == null)
            return false;

        await _unitOfWork.BorrowingTransactions.DeleteAsync(command.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}