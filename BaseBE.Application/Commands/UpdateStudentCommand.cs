namespace BaseBE.Application.Commands;

public record UpdateStudentCommand(Guid Id, string FullName, int Age, string Email);
