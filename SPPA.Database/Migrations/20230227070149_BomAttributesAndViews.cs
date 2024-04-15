using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class BomAttributesAndViews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntegerValue",
                table: "AttributeValues");

            migrationBuilder.AddColumn<bool>(
                name: "Editable",
                table: "Attributes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EnumSettings",
                table: "Attributes",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumberSettings",
                table: "Attributes",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BomViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BomViews", x => x.Id);
                    table.UniqueConstraint("AK_BomViews_OrderId_UserId", x => new { x.OrderId, x.UserId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BomViews");

            migrationBuilder.DropColumn(
                name: "Editable",
                table: "Attributes");

            migrationBuilder.DropColumn(
                name: "EnumSettings",
                table: "Attributes");

            migrationBuilder.DropColumn(
                name: "NumberSettings",
                table: "Attributes");

            migrationBuilder.AddColumn<int>(
                name: "IntegerValue",
                table: "AttributeValues",
                type: "integer",
                nullable: true);
        }
    }
}
