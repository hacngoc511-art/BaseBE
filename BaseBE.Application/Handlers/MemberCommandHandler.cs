using BaseBE.Application.Commands;
using BaseBE.Application.DTOs;
using BaseBE.Domain.Entities;
using BaseBE.Domain.Repositories;

namespace BaseBE.Application.Handlers;

public class MemberCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberDto> CreateAsync(CreateMemberCommand command, CancellationToken cancellationToken = default)
    {
        var member = new Member
        {
            FullName = command.FullName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber
        };

        await _unitOfWork.Members.AddAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MemberDto(member.Id, member.FullName, member.Email, member.PhoneNumber);
    }

    public async Task<MemberDto?> UpdateAsync(UpdateMemberCommand command, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(command.Id, cancellationToken);

        if (member == null)
            return null;

        member.FullName = command.FullName;
        member.Email = command.Email;
        member.PhoneNumber = command.PhoneNumber;

        await _unitOfWork.Members.UpdateAsync(member, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new MemberDto(member.Id, member.FullName, member.Email, member.PhoneNumber);
    }

    public async Task<bool> DeleteAsync(DeleteMemberCommand command, CancellationToken cancellationToken = default)
    {
        var member = await _unitOfWork.Members.GetByIdAsync(command.Id, cancellationToken);

        if (member == null)
            return false;

        await _unitOfWork.Members.DeleteAsync(command.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}