namespace BaseBE.Domain.Entities;

public class Member
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; }
        = new List<BorrowingTransaction>();
}