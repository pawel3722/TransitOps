
namespace TransitOps.Api.Models;

public enum ActionStatus
{
    Proposed,
    Accepted,
    Rejected,
    Completed
}

public class DisruptionAction
{
    public int Id { get; set; }

    public int DisruptionAnalysisId { get; set; }
    public DisruptionAnalysis Analysis { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;

    public ActionStatus Status { get; set; } = ActionStatus.Proposed;

    public DateTime? DecidedAt { get; set; }
}
