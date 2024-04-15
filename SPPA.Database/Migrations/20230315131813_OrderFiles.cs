using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class OrderFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsNpgsql())
            {
                migrationBuilder.Sql
                (
                    "DELETE FROM \"BomViews\"" +
                    "WHERE \"Id\" IN" +
                    "(SELECT \"B\".\"Id\"" +
                    "FROM \"BomViews\" AS \"B\"" +
                    "LEFT JOIN \"Orders\" AS \"O\"" +
                    "ON \"B\".\"OrderId\" = \"O\".\"Id\"" +
                    "WHERE \"O\".\"Id\" IS NULL);"
                );
            }
            else
            {
                throw new Exception("This migration supported only for PostgreSQL");
            }

            migrationBuilder.DropTable(
                name: "DrawingFiles");

            migrationBuilder.RenameColumn(
                name: "FileFormat",
                table: "IfcFiles",
                newName: "FileType");

            migrationBuilder.AddColumn<string>(
                name: "DrawingNumber",
                table: "IfcFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "IfcFiles",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Position",
                table: "IfcFiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "IfcFiles",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderFileAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFileAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFileAttributes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderFileAttributeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFileAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFileAttributeValues_OrderFileAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "OrderFileAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderFileAttributeValues_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BomViews_UserId",
                table: "BomViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileAttributes_OrderId",
                table: "OrderFileAttributes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileAttributeValues_AttributeId",
                table: "OrderFileAttributeValues",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileAttributeValues_OrderId",
                table: "OrderFileAttributeValues",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_BomViews_Orders_OrderId",
                table: "BomViews",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BomViews_Users_UserId",
                table: "BomViews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BomViews_Orders_OrderId",
                table: "BomViews");

            migrationBuilder.DropForeignKey(
                name: "FK_BomViews_Users_UserId",
                table: "BomViews");

            migrationBuilder.DropTable(
                name: "OrderFileAttributeValues");

            migrationBuilder.DropTable(
                name: "OrderFileAttributes");

            migrationBuilder.DropIndex(
                name: "IX_BomViews_UserId",
                table: "BomViews");

            migrationBuilder.DropColumn(
                name: "DrawingNumber",
                table: "IfcFiles");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "IfcFiles");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "IfcFiles");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "IfcFiles");

            migrationBuilder.RenameColumn(
                name: "FileType",
                table: "IfcFiles",
                newName: "FileFormat");

            migrationBuilder.CreateTable(
                name: "DrawingFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElementId = table.Column<Guid>(type: "uuid", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingFiles_Elements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrawingFiles_ElementId",
                table: "DrawingFiles",
                column: "ElementId");
        }
    }
}
