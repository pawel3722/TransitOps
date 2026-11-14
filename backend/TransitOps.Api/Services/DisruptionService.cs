using Microsoft.EntityFrameworkCore;
using TransitOps.Api.Data;
using TransitOps.Api.DTOs;
using TransitOps.Api.Models;
using TransitOps.Api.Services.Interfaces;

namespace TransitOps.Api.Services;

public sealed class DisruptionService(TransitOpsDbContext db) : IDisruptionService
{
    public async Task<IReadOnlyList<DisruptionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await db.Disruptions.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    public async Task<DisruptionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await db.Disruptions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item is null ? null : Map(item);
    }

    public async Task<DisruptionDto> CreateAsync(CreateDisruptionDto request, CancellationToken cancellationToken = default)
    {
        var item = new Disruption
        {
            Line = request.Line?.Trim() ?? string.Empty,
            Type = request.Type,
            Description = request.Description.Trim(),
            DelayMinutes = request.DelayMinutes,
            Status = DisruptionStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        db.Disruptions.Add(item);
        await db.SaveChangesAsync(cancellationToken);
        return Map(item);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDisruptionDto request, CancellationToken cancellationToken = default)
    {
        var item = await db.Disruptions.FindAsync([id], cancellationToken);
        if (item is null) return false;

        item.Line = request.Line?.Trim() ?? string.Empty;
        item.Type = request.Type;
        item.Description = request.Description.Trim();
        item.DelayMinutes = request.DelayMinutes;
        item.Status = request.Status;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await db.Disruptions.FindAsync([id], cancellationToken);
        if (item is null) return false;
        db.Disruptions.Remove(item);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static DisruptionDto Map(Disruption item) => new(
        item.Id, item.Line, item.Type, item.Description,
        item.DelayMinutes, item.Status, item.CreatedAt);
}