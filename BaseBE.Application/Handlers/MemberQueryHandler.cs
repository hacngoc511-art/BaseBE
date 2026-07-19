using BaseBE.Application.DTOs;
using BaseBE.Application.Queries;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class MemberQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MemberDto>> HandleAsync(GetMembersQuery query, CancellationToken cancellationToken = default)
    {
        var members = await _unitOfWork.Members.GetAllAsync(cancellationToken);

        return members
            .Select(x => new MemberDto(x.Id, x.FullName, x.Email, x.PhoneNumber))
            .ToList();
    }

    public async Task<MemberDto?> HandleAsync(GetMemberByIdQuery query, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(query.Id, cancellationToken);

        if (member == null)
            return null;

        return new MemberDto(member.Id, member.FullName, member.Email, member.PhoneNumber);
    }
}