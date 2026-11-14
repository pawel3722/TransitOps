namespace TransitOps.Api.Models;

public class Disruption
{
    public int Id { get; set; }

    public string Line { get; set; } = string.Empty;

    public DisruptionType Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public int? DelayMinutes { get; set; }

    public DisruptionStatus Status { get; set; } = DisruptionStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<DisruptionAnalysis> Analyses { get; set; } = [];
}

public enum DisruptionType
{
    Delay = 0,
    Breakdown = 1,
    StopClosure = 2,
    Accident = 3,
    RoadObstruction = 4,
    RouteBlocked = 5,
    InfrastructureFailure = 6,
    StaffingIssue = 7,
    PassengerIncident = 8,
    WeatherConditions = 9,
    Other = 10
}

public enum DisruptionStatus
{
    Active,
    Resolved
}