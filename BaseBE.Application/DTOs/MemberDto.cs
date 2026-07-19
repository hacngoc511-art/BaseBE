namespace BaseBE.Application.DTOs;

public record MemberDto(
    int Id,
    string FullName,
    string Email,
    string PhoneNumber
);