using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FBZSystemMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddFlaggedComics2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FlaggedComics_ComicId",
                table: "FlaggedComics",
                column: "ComicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlaggedComics_StaffUserId",
                table: "FlaggedComics",
                column: "StaffUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlaggedComics_ComicId",
                table: "FlaggedComics");

            migrationBuilder.DropIndex(
                name: "IX_FlaggedComics_StaffUserId",
                table: "FlaggedComics");
        }
    }
}
