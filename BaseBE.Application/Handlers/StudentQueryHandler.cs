using BaseBE.Application.DTOs;
using BaseBE.Application.Queries;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class StudentQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StudentDto>> HandleAsync(GetStudentsQuery query, CancellationToken cancellationToken = default)
    {
        var students = await _unitOfWork.Students.GetAllAsync(cancellationToken);
        return students.Select(s => new StudentDto(s.Id, s.FullName, s.Age, s.Email, s.CreatedAt)).ToList();
    }

    public async Task<StudentDto?> HandleAsync(GetStudentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(query.Id, cancellationToken);
        return student is null ? null : new StudentDto(student.Id, student.FullName, student.Age, student.Email, student.CreatedAt);
    }
}
