using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockFlow.Infrastructure.Data.Migrations
{
    public partial class AddSaleTypesAndMovingSale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuyAmount",
                table: "Sales",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Sales",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FreeAmount",
                table: "Sales",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SaleType",
                table: "Sales",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Sales",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovingSaleRequestedAt",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovingSaleUntil",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Sales",
                columns: ["Id", "CreatedAt", "UpdatedAt", "SoftDeleted", "SoftDeletedAt", "SaleName", "Description", "SaleType", "PercentageOff", "BuyAmount", "FreeAmount", "SalesTags"],
                values: [new Guid("6d0f5a1e-0000-4000-8000-000000000001"), new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 10, 20, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), "Moving sale", "10% off everything without another sale for customers who are moving", 2, 10, 0, 0, "[]"]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: new Guid("6d0f5a1e-0000-4000-8000-000000000001"));

            migrationBuilder.DropColumn(
                name: "BuyAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FreeAmount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SaleType",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "MovingSaleRequestedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MovingSaleUntil",
                table: "Customers");
        }
    }
}
