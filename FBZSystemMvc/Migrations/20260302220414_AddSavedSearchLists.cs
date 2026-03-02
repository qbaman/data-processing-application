using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBZSystemMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddSavedSearchLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SavedSearchLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearchLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedSearchListItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SavedSearchListId = table.Column<int>(type: "INTEGER", nullable: false),
                    ComicId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSearchListItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSearchListItems_SavedSearchLists_SavedSearchListId",
                        column: x => x.SavedSearchListId,
                        principalTable: "SavedSearchLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearchListItems_SavedSearchListId_ComicId",
                table: "SavedSearchListItems",
                columns: new[] { "SavedSearchListId", "ComicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedSearchLists_UserId_Name",
                table: "SavedSearchLists",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SavedSearchListItems");

            migrationBuilder.DropTable(
                name: "SavedSearchLists");
        }
    }
}
