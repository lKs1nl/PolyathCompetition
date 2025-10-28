using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolyathlonCompetition.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionRegistrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Competitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Competitions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompetitorId1",
                table: "CompetitionResults",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompetitionRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompetitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompetitorId = table.Column<int>(type: "INTEGER", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitionRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionRegistrations_Competitions_CompetitionId",
                        column: x => x.CompetitionId,
                        principalTable: "Competitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompetitionRegistrations_Competitors_CompetitorId",
                        column: x => x.CompetitorId,
                        principalTable: "Competitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionResults_CompetitorId1",
                table: "CompetitionResults",
                column: "CompetitorId1");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionRegistrations_CompetitionId_CompetitorId",
                table: "CompetitionRegistrations",
                columns: new[] { "CompetitionId", "CompetitorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionRegistrations_CompetitorId",
                table: "CompetitionRegistrations",
                column: "CompetitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompetitionResults_Competitors_CompetitorId1",
                table: "CompetitionResults",
                column: "CompetitorId1",
                principalTable: "Competitors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetitionResults_Competitors_CompetitorId1",
                table: "CompetitionResults");

            migrationBuilder.DropTable(
                name: "CompetitionRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_CompetitionResults_CompetitorId1",
                table: "CompetitionResults");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "Competitions");

            migrationBuilder.DropColumn(
                name: "CompetitorId1",
                table: "CompetitionResults");
        }
    }
}
