using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TransitOps.Api.Data;
using TransitOps.Api.Models;
using TransitOps.Api.Services;
using TransitOps.Api.Services.Interfaces;
using Xunit;

namespace TransitOps.Tests;

public class DisruptionAnalysisServiceTests
{
    [Fact]
    public async Task ProposedAction_CanBeAcceptedAndCompleted()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateDb(connection);
        await db.Database.EnsureCreatedAsync();

        var action = await AddActionAsync(db);
        var service = new DisruptionAnalysisService(db, new FakeAiAnalysisService());

        var accepted = await service.UpdateActionStatusAsync(action.Id, "Accepted");
        Assert.Equal("Accepted", accepted?.Status);

        var completed = await service.UpdateActionStatusAsync(action.Id, "Completed");
        Assert.Equal("Completed", completed?.Status);
        Assert.NotNull((await db.DisruptionActions.FindAsync(action.Id))?.DecidedAt);
    }

    [Fact]
    public async Task ProposedAction_CannotBeCompletedDirectly()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateDb(connection);
        await db.Database.EnsureCreatedAsync();

        var action = await AddActionAsync(db);
        var service = new DisruptionAnalysisService(db, new FakeAiAnalysisService());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateActionStatusAsync(action.Id, "Completed"));

        Assert.Equal(ActionStatus.Proposed, action.Status);
    }

    [Fact]
    public async Task RepeatedAnalysis_CreatesSeparateHistoryEntries()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateDb(connection);
        await db.Database.EnsureCreatedAsync();

        var disruption = new Disruption
        {
            Line = "12",
            Type = DisruptionType.Delay,
            Description = "Opóźnienie kursu",
            Status = DisruptionStatus.Active
        };
        db.Disruptions.Add(disruption);
        await db.SaveChangesAsync();

        var service = new DisruptionAnalysisService(db, new FakeAiAnalysisService());
        await service.AnalyzeAsync(disruption.Id);
        await service.AnalyzeAsync(disruption.Id);

        var history = await service.GetHistoryAsync(disruption.Id);
        Assert.Equal(2, history.Count);
        Assert.NotEqual(history[0].Id, history[1].Id);
        Assert.All(history, analysis => Assert.Single(analysis.Actions));
    }

    private static TransitOpsDbContext CreateDb(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<TransitOpsDbContext>()
            .UseSqlite(connection)
            .Options;
        return new TransitOpsDbContext(options);
    }

    private static async Task<DisruptionAction> AddActionAsync(TransitOpsDbContext db)
    {
        var disruption = new Disruption
        {
            Line = "12",
            Type = DisruptionType.Delay,
            Description = "Opóźnienie kursu"
        };
        var analysis = new DisruptionAnalysis
        {
            Disruption = disruption,
            Priority = IncidentPriority.Medium,
            Summary = "Opóźnienie wymaga sprawdzenia"
        };
        var action = new DisruptionAction
        {
            Analysis = analysis,
            Title = "Kontakt z prowadzącym",
            Reason = "Ustalenie przyczyny opóźnienia"
        };
        db.DisruptionActions.Add(action);
        await db.SaveChangesAsync();
        return action;
    }

    private sealed class FakeAiAnalysisService : IAiAnalysisService
    {
        public Task<AiAnalysisResult> AnalyzeAsync(
            Disruption disruption,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new AiAnalysisResult(
                IncidentPriority.Medium,
                "Należy ustalić przyczynę opóźnienia.",
                [new AiRecommendation("Kontakt z prowadzącym", "Potwierdzenie sytuacji na trasie.")]));
    }
}
