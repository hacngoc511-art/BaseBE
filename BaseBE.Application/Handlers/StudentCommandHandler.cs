using BaseBE.Application.Commands;
using BaseBE.Application.DTOs;
using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class StudentCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StudentDto> CreateAsync(CreateStudentCommand command, CancellationToken cancellationToken = default)
    {
        var student = new Student
        {
            FullName = command.FullName,
            Age = command.Age,
            Email = command.Email
        };

        await _unitOfWork.Students.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentDto(student.Id, student.FullName, student.Age, student.Email, student.CreatedAt);
    }

    public async Task<StudentDto?> UpdateAsync(UpdateStudentCommand command, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(command.Id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        student.FullName = command.FullName;
        student.Age = command.Age;
        student.Email = command.Email;

        await _unitOfWork.Students.UpdateAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new StudentDto(student.Id, student.FullName, student.Age, student.Email, student.CreatedAt);
    }

    public async Task<bool> DeleteAsync(DeleteStudentCommand command, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(command.Id, cancellationToken);
        if (student is null)
        {
            return false;
        }

        await _unitOfWork.Students.DeleteAsync(command.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
