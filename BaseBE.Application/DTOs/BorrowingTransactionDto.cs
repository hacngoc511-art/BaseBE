namespace BaseBE.Application.DTOs;

public record BorrowingTransactionDto(
    int Id,
    int MemberId,
    int BookId,
    DateTime BorrowDate,
    DateTime? ReturnDate,
    bool IsReturned
);