using BaseBE.Application.Commands;
using BaseBE.Application.Handlers;
using BaseBE.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BaseBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MembersController : ControllerBase
{
    private readonly MemberQueryHandler _queryHandler;
    private readonly MemberCommandHandler _commandHandler;

    public MembersController(
        MemberQueryHandler queryHandler,
        MemberCommandHandler commandHandler)
    {
        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(new GetMembersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(
            new GetMemberByIdQuery { Id = id },
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMemberCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _commandHandler.CreateAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateMemberCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;

        var result = await _commandHandler.UpdateAsync(command, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var success = await _commandHandler.DeleteAsync(
            new DeleteMemberCommand { Id = id },
            cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }
}