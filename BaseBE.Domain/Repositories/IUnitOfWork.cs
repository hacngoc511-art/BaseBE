namespace BaseBE.Domain.Repositories;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
