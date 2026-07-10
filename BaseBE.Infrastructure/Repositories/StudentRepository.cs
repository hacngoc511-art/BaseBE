using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Students.OrderBy(x => x.FullName).ToListAsync(cancellationToken);

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        await _context.Students.AddAsync(student, cancellationToken);
    }

    public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _context.Students.Update(student);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Students.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is not null)
        {
            _context.Students.Remove(entity);
        }
    }
}
