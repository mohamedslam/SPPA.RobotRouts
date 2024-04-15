using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class OrderFileElementRef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderFileElementRefs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ElementId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderFileElementRefs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderFileElementRefs_Elements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "Elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderFileElementRefs_OrderFiles_FileId",
                        column: x => x.FileId,
                        principalTable: "OrderFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderFileElementRefs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileElementRefs_ElementId",
                table: "OrderFileElementRefs",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileElementRefs_FileId",
                table: "OrderFileElementRefs",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderFileElementRefs_OrderId",
                table: "OrderFileElementRefs",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderFileElementRefs");
        }
    }
}
