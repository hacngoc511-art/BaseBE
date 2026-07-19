namespace BaseBE.Application.Commands;

public class UpdateBorrowingTransactionCommand
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int BookId { get; set; }

    public DateTime BorrowDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public bool IsReturned { get; set; }
}