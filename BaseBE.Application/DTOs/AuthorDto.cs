namespace BaseBE.Application.DTOs;

public record AuthorDto(
    int Id,
    string Name,
    string? Biography
);