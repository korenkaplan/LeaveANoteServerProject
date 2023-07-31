using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveANoteServerProject.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnUnmatchedReportIsMatched : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUnmatched",
                table: "UnmatchedReports",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUnmatched",
                table: "UnmatchedReports");
        }
    }
}
