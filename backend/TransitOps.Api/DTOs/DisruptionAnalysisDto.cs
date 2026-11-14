
namespace TransitOps.Api.DTOs;

public record DisruptionActionDto(
    int Id,
    string Title,
    string Reason,
    string Status
);

public record DisruptionAnalysisDto(
    int Id,
    int DisruptionId,
    string Priority,
    string Summary,
    DateTime CreatedAt,
    List<DisruptionActionDto> Actions
);
