namespace BaseBE.Application.Commands;

public record CreateStudentCommand(string FullName, int Age, string Email);
