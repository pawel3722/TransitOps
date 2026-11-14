using TransitOps.Api.Models;

namespace TransitOps.Api.Services.Interfaces;

public interface IAiAnalysisService
{
    Task<AiAnalysisResult> AnalyzeAsync(
        Disruption disruption,
        CancellationToken cancellationToken = default);
}

public record AiAnalysisResult(
    IncidentPriority Priority,
    string Summary,
    List<AiRecommendation> Recommendations);

public record AiRecommendation(string Title, string Reason);