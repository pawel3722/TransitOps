using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TransitOps.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddDisruptionAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisruptionAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisruptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisruptionAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisruptionAnalyses_Disruptions_DisruptionId",
                        column: x => x.DisruptionId,
                        principalTable: "Disruptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisruptionActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisruptionAnalysisId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DecidedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisruptionActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisruptionActions_DisruptionAnalyses_DisruptionAnalysisId",
                        column: x => x.DisruptionAnalysisId,
                        principalTable: "DisruptionAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisruptionActions_DisruptionAnalysisId",
                table: "DisruptionActions",
                column: "DisruptionAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_DisruptionAnalyses_DisruptionId",
                table: "DisruptionAnalyses",
                column: "DisruptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisruptionActions");

            migrationBuilder.DropTable(
                name: "DisruptionAnalyses");
        }
    }
}
