using Microsoft.AspNetCore.Mvc;
using TodoYonetim.Api.DTOs;
using TodoYonetim.Api.Services;

namespace TodoYonetim.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _service;

    public TodosController(ITodoService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TodoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TodoResponse>>> GetAll(
        [FromQuery] TodoQueryRequest query,
        CancellationToken cancellationToken)
    {
        return Ok(await _service.GetAllAsync(query, cancellationToken));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        return Ok(await _service.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> Create(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> Update(int id, UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _service.UpdateAsync(id, request, cancellationToken));
    }

    [HttpPatch("{id:int}/toggle")]
    [ProducesResponseType(typeof(TodoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoResponse>> Toggle(int id, CancellationToken cancellationToken)
    {
        return Ok(await _service.ToggleAsync(id, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
