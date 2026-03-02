using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBZSystemMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResultCount",
                table: "SearchAnalyticsEvents",
                newName: "Truncated");

            migrationBuilder.RenameColumn(
                name: "QueryText",
                table: "SearchAnalyticsEvents",
                newName: "QuerySignature");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "SearchAnalyticsEvents",
                newName: "OccurredUtc");

            migrationBuilder.AddColumn<int>(
                name: "CountedResults",
                table: "SearchAnalyticsEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalResults",
                table: "SearchAnalyticsEvents",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ComicResultStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ComicId = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComicResultStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueryStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Signature = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueryStats", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SearchAnalyticsEvents_OccurredUtc",
                table: "SearchAnalyticsEvents",
                column: "OccurredUtc");

            migrationBuilder.CreateIndex(
                name: "IX_ComicResultStats_ComicId",
                table: "ComicResultStats",
                column: "ComicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueryStats_Signature",
                table: "QueryStats",
                column: "Signature",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComicResultStats");

            migrationBuilder.DropTable(
                name: "QueryStats");

            migrationBuilder.DropIndex(
                name: "IX_SearchAnalyticsEvents_OccurredUtc",
                table: "SearchAnalyticsEvents");

            migrationBuilder.DropColumn(
                name: "CountedResults",
                table: "SearchAnalyticsEvents");

            migrationBuilder.DropColumn(
                name: "TotalResults",
                table: "SearchAnalyticsEvents");

            migrationBuilder.RenameColumn(
                name: "Truncated",
                table: "SearchAnalyticsEvents",
                newName: "ResultCount");

            migrationBuilder.RenameColumn(
                name: "QuerySignature",
                table: "SearchAnalyticsEvents",
                newName: "QueryText");

            migrationBuilder.RenameColumn(
                name: "OccurredUtc",
                table: "SearchAnalyticsEvents",
                newName: "CreatedAtUtc");
        }
    }
}
