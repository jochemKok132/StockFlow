using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockFlow.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMovingSaleApprovalAndBundles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BundlePrice",
                table: "Sales",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MovingSaleHouseNumber",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovingSalePostalCode",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MovingSaleReviewedAt",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovingSaleReviewedBy",
                table: "Customers",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MovingSaleStatus",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BundlePrice",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "MovingSaleHouseNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MovingSalePostalCode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MovingSaleReviewedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MovingSaleReviewedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MovingSaleStatus",
                table: "Customers");
        }
    }
}
