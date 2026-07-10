namespace BaseBE.Application.DTOs;

public record StudentDto(Guid Id, string FullName, int Age, string Email, DateTime CreatedAt);
