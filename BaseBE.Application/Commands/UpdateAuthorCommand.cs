namespace BaseBE.Application.Commands;

public class UpdateAuthorCommand
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }
}