namespace BaseBE.Application.Commands;

public class CreateAuthorCommand
{
    public string Name { get; set; } = string.Empty;

    public string? Biography { get; set; }
}