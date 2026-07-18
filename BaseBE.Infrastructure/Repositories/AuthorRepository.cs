using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Author?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Authors
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Author>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Authors.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        await _context.Authors.AddAsync(author, cancellationToken);
    }

    public Task UpdateAsync(Author author, CancellationToken cancellationToken = default)
    {
        _context.Authors.Update(author);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await GetByIdAsync(id, cancellationToken);

        if (author != null)
        {
            _context.Authors.Remove(author);
        }
    }
}