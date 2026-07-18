namespace BaseBE.Domain.Repositories;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }

    IAuthorRepository Authors { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
