using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class UserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrawingNumber",
                table: "OrderFiles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "OrderFiles");

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    InterfaceLanguage = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    ReportLanguage = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.AddColumn<string>(
                name: "DrawingNumber",
                table: "OrderFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "OrderFiles",
                type: "text",
                nullable: true);
        }
    }
}
