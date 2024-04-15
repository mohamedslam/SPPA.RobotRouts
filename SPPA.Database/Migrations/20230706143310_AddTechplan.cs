using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPPA.Database.Migrations
{
    public partial class AddTechplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TechoperationTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OperationLogicalType = table.Column<string>(type: "text", nullable: false),
                    OperationObject = table.Column<string>(type: "text", nullable: true),
                    MaterialUsage = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechoperationTemplates", x => new { x.WorkspaceId, x.Id });
                    table.ForeignKey(
                        name: "FK_TechoperationTemplates_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechplanViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechplanViews", x => x.Id);
                    table.UniqueConstraint("AK_TechplanViews_OrderId_UserId", x => new { x.OrderId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TechplanViews_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechplanViews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Techproceses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BomElementId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsTemplateMatch = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techproceses", x => new { x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_Techproceses_Elements_BomElementId",
                        column: x => x.BomElementId,
                        principalTable: "Elements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Techproceses_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechprocessTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Techoperations = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechprocessTemplates", x => new { x.WorkspaceId, x.Id });
                    table.ForeignKey(
                        name: "FK_TechprocessTemplates_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Techoperations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TechprocessId = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationLogicalType = table.Column<string>(type: "text", nullable: false),
                    OperationObject = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ListOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Techoperations", x => new { x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_Techoperations_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Techoperations_Techproceses_OrderId_TechprocessId",
                        columns: x => new { x.OrderId, x.TechprocessId },
                        principalTable: "Techproceses",
                        principalColumns: new[] { "OrderId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechoperationMaterialUsages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    TechoperationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Placeholder = table.Column<string>(type: "text", nullable: false),
                    MaterialName = table.Column<string>(type: "text", nullable: false),
                    AmountAttr = table.Column<double>(type: "double precision", nullable: true),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Formula = table.Column<string>(type: "text", nullable: true),
                    CalculatedResult = table.Column<double>(type: "double precision", nullable: true),
                    ManualResult = table.Column<double>(type: "double precision", nullable: true),
                    ResultUnit = table.Column<string>(type: "text", nullable: true),
                    IsBilletMaterial = table.Column<bool>(type: "boolean", nullable: true),
                    CuttingSheetId = table.Column<Guid>(type: "uuid", nullable: true),
                    ListOrder = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechoperationMaterialUsages", x => new { x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_TechoperationMaterialUsages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechoperationMaterialUsages_Techoperations_OrderId_Techoper~",
                        columns: x => new { x.OrderId, x.TechoperationId },
                        principalTable: "Techoperations",
                        principalColumns: new[] { "OrderId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechoperationMaterialUsages_OrderId_TechoperationId",
                table: "TechoperationMaterialUsages",
                columns: new[] { "OrderId", "TechoperationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Techoperations_OrderId_TechprocessId",
                table: "Techoperations",
                columns: new[] { "OrderId", "TechprocessId" });

            migrationBuilder.CreateIndex(
                name: "IX_TechplanViews_UserId",
                table: "TechplanViews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Techproceses_BomElementId",
                table: "Techproceses",
                column: "BomElementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechoperationMaterialUsages");

            migrationBuilder.DropTable(
                name: "TechoperationTemplates");

            migrationBuilder.DropTable(
                name: "TechplanViews");

            migrationBuilder.DropTable(
                name: "TechprocessTemplates");

            migrationBuilder.DropTable(
                name: "Techoperations");

            migrationBuilder.DropTable(
                name: "Techproceses");
        }
    }
}
