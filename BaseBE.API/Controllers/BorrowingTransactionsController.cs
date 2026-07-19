using BaseBE.Application.Commands;
using BaseBE.Application.Handlers;
using BaseBE.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BaseBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BorrowingTransactionsController : ControllerBase
{
    private readonly BorrowingTransactionCommandHandler _commandHandler;
    private readonly BorrowingTransactionQueryHandler _queryHandler;

    public BorrowingTransactionsController(
        BorrowingTransactionCommandHandler commandHandler,
        BorrowingTransactionQueryHandler queryHandler)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(
            new GetBorrowingTransactionsQuery(),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(
            new GetBorrowingTransactionByIdQuery
            {
                Id = id
            },
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBorrowingTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _commandHandler.CreateAsync(command, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateBorrowingTransactionCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        var result = await _commandHandler.UpdateAsync(command, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _commandHandler.DeleteAsync(
            new DeleteBorrowingTransactionCommand
            {
                Id = id
            },
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }
}