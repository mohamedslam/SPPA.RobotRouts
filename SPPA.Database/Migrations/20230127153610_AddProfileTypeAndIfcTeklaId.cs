using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class AddProfileTypeAndIfcTeklaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IfcTeklaId",
                table: "Elements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileType",
                table: "Elements",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IfcTeklaId",
                table: "Elements");

            migrationBuilder.DropColumn(
                name: "ProfileType",
                table: "Elements");
        }
    }
}
