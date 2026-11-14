
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TransitOps.Api.Services;
using TransitOps.Api.Services.Interfaces;

namespace TransitOps.Api.Controllers;

[ApiController]
[Route("api")]
public class DisruptionAnalysesController : ControllerBase
{
    private readonly IDisruptionAnalysisService _service;
    private readonly ILogger<DisruptionAnalysesController> _logger;

    public DisruptionAnalysesController(
        IDisruptionAnalysisService service,
        ILogger<DisruptionAnalysesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("disruptions/{id:int}/analyze")]
    public async Task<IActionResult> Analyze(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.AnalyzeAsync(
                id, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (InvalidOperationException ex)
            when (ex.Message == "Cannot analyze a resolved incident.")
        {
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex) when (
            ex is HttpRequestException or
            JsonException or
            InvalidOperationException or
            KeyNotFoundException)
        {
            _logger.LogError(ex, "AI analysis failed for {Id}", id);

            return StatusCode(502, new
            {
                error = "AI analysis is temporarily unavailable."
            });
        }
    }

    [HttpGet("disruptions/{id:int}/analyses")]
    public async Task<IActionResult> History(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetHistoryAsync(
            id, cancellationToken);

        return Ok(result);
    }

    [HttpPatch("actions/{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateActionStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateActionStatusAsync(
                id, request.Status, cancellationToken);

            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}

public record UpdateActionStatusRequest(string Status);
