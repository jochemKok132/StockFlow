using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockFlow.Infrastructure.Data.Migrations
{
    public partial class AddReceiptsAndReturns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReturnedAmount",
                table: "RegisterItemDetails",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptId",
                table: "CashRegisterLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE "CashRegisterLogs"
                SET "ReceiptId" = CAST(9000000000 + (
                    SELECT COUNT(*) FROM "CashRegisterLogs" AS "Other"
                    WHERE "Other"."CreatedAt" < "CashRegisterLogs"."CreatedAt"
                       OR ("Other"."CreatedAt" = "CashRegisterLogs"."CreatedAt" AND "Other"."Id" <= "CashRegisterLogs"."Id")
                ) AS TEXT);
                """);

            migrationBuilder.CreateIndex(
                name: "IX_CashRegisterLogs_ReceiptId",
                table: "CashRegisterLogs",
                column: "ReceiptId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CashRegisterLogs_ReceiptId",
                table: "CashRegisterLogs");

            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "CashRegisterLogs");

            migrationBuilder.DropColumn(
                name: "ReturnedAmount",
                table: "RegisterItemDetails");
        }
    }
}
