using Microsoft.EntityFrameworkCore;
using TransitOps.Api.Models;

namespace TransitOps.Api.Data;

public class TransitOpsDbContext : DbContext
{
    public TransitOpsDbContext(
        DbContextOptions<TransitOpsDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Disruption> Disruptions => Set<Disruption>();
    public DbSet<DisruptionAnalysis> DisruptionAnalyses => Set<DisruptionAnalysis>();
    public DbSet<DisruptionAction> DisruptionActions => Set<DisruptionAction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disruption>(entity =>
        {
            entity.Property(d => d.Line)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(d => d.Description)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(d => d.Type)
                .HasConversion<string>();

            entity.Property(d => d.Status)
                .HasConversion<string>();
        });

        modelBuilder.Entity<DisruptionAnalysis>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(x => x.Summary)
                .IsRequired()
                .HasMaxLength(2000);

            entity.HasOne(x => x.Disruption)
                .WithMany(x => x.Analyses)
                .HasForeignKey(x => x.DisruptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DisruptionAction>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasOne(x => x.Analysis)
                .WithMany(x => x.Actions)
                .HasForeignKey(x => x.DisruptionAnalysisId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}