using TransitOps.Api.DTOs;

namespace TransitOps.Api.Services.Interfaces;

public interface IDisruptionAnalysisService
{
    Task<DisruptionAnalysisDto?> AnalyzeAsync(
        int disruptionId,
        CancellationToken cancellationToken = default);

    Task<List<DisruptionAnalysisDto>> GetHistoryAsync(
        int disruptionId,
        CancellationToken cancellationToken = default);

    Task<DisruptionActionDto?> UpdateActionStatusAsync(
        int actionId,
        string status,
        CancellationToken cancellationToken = default);
}