using BaseBE.Domain.Entities;

namespace BaseBE.Domain.Repositories;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
