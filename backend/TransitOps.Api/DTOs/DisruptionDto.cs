using TransitOps.Api.Models;

namespace TransitOps.Api.DTOs;

public sealed record DisruptionDto(
    int Id,
    string Line,
    DisruptionType Type,
    string Description,
    int? DelayMinutes,
    DisruptionStatus Status,
    DateTime CreatedAt);