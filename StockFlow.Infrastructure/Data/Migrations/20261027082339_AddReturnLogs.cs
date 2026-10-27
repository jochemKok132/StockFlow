using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReturnLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceiptId = table.Column<string>(type: "TEXT", nullable: false),
                    CashRegisterLogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CustomerId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RefundAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    PointsDeducted = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnLogs_CashRegisterLogs_CashRegisterLogId",
                        column: x => x.CashRegisterLogId,
                        principalTable: "CashRegisterLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnLogs_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReturnLogs_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnLogLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReturnLogId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProductId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    PriceAtTimeOfSale = table.Column<decimal>(type: "TEXT", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnLogLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnLogLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnLogLines_ReturnLogs_ReturnLogId",
                        column: x => x.ReturnLogId,
                        principalTable: "ReturnLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogLines_ProductId",
                table: "ReturnLogLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogLines_ReturnLogId",
                table: "ReturnLogLines",
                column: "ReturnLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogs_CashRegisterLogId",
                table: "ReturnLogs",
                column: "CashRegisterLogId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogs_CustomerId",
                table: "ReturnLogs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogs_EmployeeId",
                table: "ReturnLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnLogs_ReceiptId",
                table: "ReturnLogs",
                column: "ReceiptId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnLogLines");

            migrationBuilder.DropTable(
                name: "ReturnLogs");
        }
    }
}
