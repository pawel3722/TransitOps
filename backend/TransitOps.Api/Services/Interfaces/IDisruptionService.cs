using TransitOps.Api.DTOs;

namespace TransitOps.Api.Services.Interfaces;

public interface IDisruptionService
{
    Task<IReadOnlyList<DisruptionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DisruptionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DisruptionDto> CreateAsync(CreateDisruptionDto request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateDisruptionDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}