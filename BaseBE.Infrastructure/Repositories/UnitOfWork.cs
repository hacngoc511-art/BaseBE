using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;

namespace BaseBE.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;

        Students = new StudentRepository(_context);
        Authors = new AuthorRepository(_context);
    }

    public IStudentRepository Students { get; }

    public IAuthorRepository Authors { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}