using CardiacPatientMonitoring.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardiacPatientMonitoring.Api.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260826210000_AddOrderMonetaryFields")]
public partial class AddOrderMonetaryFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "OrderTotal",
            table: "Orders",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "LineTotal",
            table: "OrderItems",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "UnitPrice",
            table: "Products",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.UpdateData(
            table: "Products",
            keyColumn: "Id",
            keyValue: 1,
            column: "UnitPrice",
            value: 125.00m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "OrderTotal", table: "Orders");
        migrationBuilder.DropColumn(name: "LineTotal", table: "OrderItems");
        migrationBuilder.DropColumn(name: "UnitPrice", table: "Products");
    }
}
