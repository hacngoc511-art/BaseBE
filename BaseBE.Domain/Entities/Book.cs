namespace BaseBE.Domain.Entities;

public class Book
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int AuthorId { get; set; }

    public Author? Author { get; set; }
}