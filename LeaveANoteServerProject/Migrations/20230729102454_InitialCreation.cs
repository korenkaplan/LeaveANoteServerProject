using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveANoteServerProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accidents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HittingDriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HittingCarNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HittingDriverPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReporterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReporterPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageSource = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAnonymous = table.Column<bool>(type: "bit", nullable: false),
                    IsIdentify = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accidents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accidents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UnmatchedReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DamagedCarNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccidentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnmatchedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnmatchedReports_Accidents_AccidentId",
                        column: x => x.AccidentId,
                        principalTable: "Accidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accidents_UserId",
                table: "Accidents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UnmatchedReports_AccidentId",
                table: "UnmatchedReports",
                column: "AccidentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnmatchedReports");

            migrationBuilder.DropTable(
                name: "Accidents");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
