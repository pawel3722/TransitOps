
using Microsoft.EntityFrameworkCore;
using TransitOps.Api.Data;
using TransitOps.Api.DTOs;
using TransitOps.Api.Models;
using TransitOps.Api.Services.Interfaces;

namespace TransitOps.Api.Services;

public sealed class DisruptionAnalysisService
    : IDisruptionAnalysisService
{
    private readonly TransitOpsDbContext _db;
    private readonly IAiAnalysisService _ai;

    public DisruptionAnalysisService(
        TransitOpsDbContext db,
        IAiAnalysisService ai)
    {
        _db = db;
        _ai = ai;
    }

    public async Task<DisruptionAnalysisDto?> AnalyzeAsync(
        int disruptionId,
        CancellationToken cancellationToken = default)
    {
        var disruption = await _db.Disruptions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                d => d.Id == disruptionId,
                cancellationToken);

        if (disruption is null)
            return null;

        if (disruption.Status == DisruptionStatus.Resolved)
            throw new InvalidOperationException(
                "Cannot analyze a resolved incident.");

        var result = await _ai.AnalyzeAsync(
            disruption, cancellationToken);

        var analysis = new DisruptionAnalysis
        {
            DisruptionId = disruptionId,
            Priority = result.Priority,
            Summary = result.Summary,
            Actions = result.Recommendations.Select(r =>
                new DisruptionAction
                {
                    Title = r.Title,
                    Reason = r.Reason,
                    Status = ActionStatus.Proposed
                }).ToList()
        };

        _db.DisruptionAnalyses.Add(analysis);
        await _db.SaveChangesAsync(cancellationToken);

        return Map(analysis);
    }

    public async Task<List<DisruptionAnalysisDto>> GetHistoryAsync(
        int disruptionId,
        CancellationToken cancellationToken = default)
    {
        var analyses = await _db.DisruptionAnalyses
            .AsNoTracking()
            .Include(a => a.Actions)
            .Where(a => a.DisruptionId == disruptionId)
            .OrderByDescending(a => a.CreatedAt)
            .ThenByDescending(a => a.Id)
            .ToListAsync(cancellationToken);

        return analyses.Select(Map).ToList();
    }

    public async Task<DisruptionActionDto?> UpdateActionStatusAsync(
        int actionId,
        string status,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<ActionStatus>(
                status, true, out var newStatus) ||
            !Enum.IsDefined(newStatus))
        {
            throw new ArgumentException("Invalid action status.");
        }

        var action = await _db.DisruptionActions
            .FirstOrDefaultAsync(
                a => a.Id == actionId,
                cancellationToken);

        if (action is null)
            return null;

        var validTransition = (action.Status, newStatus) switch
        {
            (ActionStatus.Proposed, ActionStatus.Accepted) => true,
            (ActionStatus.Proposed, ActionStatus.Rejected) => true,
            (ActionStatus.Accepted, ActionStatus.Completed) => true,
            _ => false
        };

        if (!validTransition)
            throw new InvalidOperationException(
                "Invalid action status transition.");

        action.Status = newStatus;
        action.DecidedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);

        return MapAction(action);
    }

    private static DisruptionAnalysisDto Map(
        DisruptionAnalysis analysis) =>
        new(
            analysis.Id,
            analysis.DisruptionId,
            analysis.Priority.ToString(),
            analysis.Summary,
            analysis.CreatedAt,
            analysis.Actions
                .OrderBy(a => a.Id)
                .Select(MapAction)
                .ToList()
        );

    private static DisruptionActionDto MapAction(
        DisruptionAction action) =>
        new(
            action.Id,
            action.Title,
            action.Reason,
            action.Status.ToString()
        );
}
