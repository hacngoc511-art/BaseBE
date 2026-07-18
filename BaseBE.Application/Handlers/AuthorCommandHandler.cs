using BaseBE.Application.DTOs;
using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class AuthorCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthorCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthorDto> CreateAsync(AuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = new Author
        {
            Name = dto.Name,
            Biography = dto.Biography
        };

        await _unitOfWork.Authors.AddAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthorDto(author.Id, author.Name, author.Biography);
    }

    public async Task<AuthorDto?> UpdateAsync(int id, AuthorDto dto, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);

        if (author == null)
            return null;

        author.Name = dto.Name;
        author.Biography = dto.Biography;

        await _unitOfWork.Authors.UpdateAsync(author, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthorDto(author.Id, author.Name, author.Biography);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var author = await _unitOfWork.Authors.GetByIdAsync(id, cancellationToken);

        if (author == null)
            return false;

        await _unitOfWork.Authors.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}