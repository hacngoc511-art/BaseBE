using BaseBE.Application.DTOs;
using BaseBE.Application.Queries;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class AuthorQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AuthorDto>> HandleAsync(GetAuthorsQuery query, CancellationToken cancellationToken = default)
    {
        var authors = await _unitOfWork.Authors.GetAllAsync(cancellationToken);

        return authors
            .Select(a => new AuthorDto(a.Id, a.Name, a.Biography))
            .ToList();
    }

    public async Task<AuthorDto?> HandleAsync(GetAuthorByIdQuery query, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(query.Id, cancellationToken);

        if (author == null)
            return null;

        return new AuthorDto(author.Id, author.Name, author.Biography);
    }
}