using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniShop.DataAccess.Migrations
{
    public partial class AddUserToProductReviewDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ProductReviews");
        }
    }
}
