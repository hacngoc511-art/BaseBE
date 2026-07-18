using BaseBE.Application.DTOs;
using BaseBE.Application.Handlers;
using BaseBE.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BaseBE.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly AuthorQueryHandler _queryHandler;
    private readonly AuthorCommandHandler _commandHandler;

    public AuthorsController(
        AuthorQueryHandler queryHandler,
        AuthorCommandHandler commandHandler)
    {
        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    // GET: api/authors
    [HttpGet]
    public async Task<IActionResult> GetAuthors(CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(new GetAuthorsQuery(), cancellationToken);
        return Ok(result);
    }

    // GET: api/authors/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(int id, CancellationToken cancellationToken)
    {
        var result = await _queryHandler.HandleAsync(
            new GetAuthorByIdQuery { Id = id },
            cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // POST: api/authors
    [HttpPost]
    public async Task<IActionResult> CreateAuthor(
        [FromBody] AuthorDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _commandHandler.CreateAsync(dto, cancellationToken);
        return Ok(result);
    }

    // PUT: api/authors/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor(
        int id,
        [FromBody] AuthorDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _commandHandler.UpdateAsync(id, dto, cancellationToken);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // DELETE: api/authors/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(
        int id,
        CancellationToken cancellationToken)
    {
        var success = await _commandHandler.DeleteAsync(id, cancellationToken);

        if (!success)
            return NotFound();

        return NoContent();
    }
}