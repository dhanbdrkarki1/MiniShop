using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShop.DataAccess.Migrations
{
    public partial class OrderColumnUpdatedToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingDate",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "PaymentDueDate",
                table: "Orders",
                newName: "PaymentMethod");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "Orders",
                newName: "PaymentDueDate");

            migrationBuilder.AddColumn<string>(
                name: "PaymentDate",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShippingDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
