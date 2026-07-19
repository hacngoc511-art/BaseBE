using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;
using BaseBE.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseBE.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Member>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Members.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Member member, CancellationToken cancellationToken = default)
    {
        await _context.Members.AddAsync(member, cancellationToken);
    }

    public Task UpdateAsync(Member member, CancellationToken cancellationToken = default)
    {
        _context.Members.Update(member);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var member = await GetByIdAsync(id, cancellationToken);

        if (member != null)
        {
            _context.Members.Remove(member);
        }
    }
}