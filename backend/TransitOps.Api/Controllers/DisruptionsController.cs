using Microsoft.AspNetCore.Mvc;
using TransitOps.Api.DTOs;
using TransitOps.Api.Services.Interfaces;

namespace TransitOps.Api.Controllers;

[ApiController]
[Route("api/disruptions")]
public sealed class DisruptionsController(IDisruptionService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DisruptionDto>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DisruptionDto>> GetById(int id, CancellationToken ct)
    {
        var item = await service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DisruptionDto>> Create(CreateDisruptionDto request, CancellationToken ct)
    {
        var item = await service.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDisruptionDto request, CancellationToken ct)
        => await service.UpdateAsync(id, request, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}