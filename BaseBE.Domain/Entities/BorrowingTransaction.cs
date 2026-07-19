namespace BaseBE.Domain.Entities;

public class BorrowingTransaction
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public Member Member { get; set; } = null!;

    public int BookId { get; set; }

    public Book Book { get; set; } = null!;

    public DateTime BorrowDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    public bool IsReturned { get; set; }
}