using BaseBE.Application.Commands;
using BaseBE.Application.Handlers;
using BaseBE.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BaseBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly StudentCommandHandler _commandHandler;
    private readonly StudentQueryHandler _queryHandler;

    public StudentsController(StudentCommandHandler commandHandler, StudentQueryHandler queryHandler)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetStudentsQuery();
        var result = await _queryHandler.HandleAsync(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStudentByIdQuery(id);
        var result = await _queryHandler.HandleAsync(query, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentCommand command, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.CreateAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route id does not match body id.");
        }

        var result = await _commandHandler.UpdateAsync(command, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _commandHandler.DeleteAsync(new DeleteStudentCommand(id), cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
