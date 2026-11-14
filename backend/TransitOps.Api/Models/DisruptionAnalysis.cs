
namespace TransitOps.Api.Models;

public enum IncidentPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class DisruptionAnalysis
{
    public int Id { get; set; }

    public int DisruptionId { get; set; }
    public Disruption Disruption { get; set; } = null!;

    public IncidentPriority Priority { get; set; }
    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<DisruptionAction> Actions { get; set; } = new();
}
