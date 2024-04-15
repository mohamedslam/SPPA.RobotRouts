using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class RenameIfcFiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IfcFiles_Orders_OrderId",
                table: "IfcFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IfcFiles",
                table: "IfcFiles");

            migrationBuilder.RenameTable(
                name: "IfcFiles",
                newName: "OrderFiles");

            migrationBuilder.RenameIndex(
                name: "IX_IfcFiles_OrderId",
                table: "OrderFiles",
                newName: "IX_OrderFiles_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderFiles",
                table: "OrderFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderFiles_Orders_OrderId",
                table: "OrderFiles",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderFiles_Orders_OrderId",
                table: "OrderFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderFiles",
                table: "OrderFiles");

            migrationBuilder.RenameTable(
                name: "OrderFiles",
                newName: "IfcFiles");

            migrationBuilder.RenameIndex(
                name: "IX_OrderFiles_OrderId",
                table: "IfcFiles",
                newName: "IX_IfcFiles_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IfcFiles",
                table: "IfcFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IfcFiles_Orders_OrderId",
                table: "IfcFiles",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
