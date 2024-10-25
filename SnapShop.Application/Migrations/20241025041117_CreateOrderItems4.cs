using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnapShop.Application.Migrations
{
    /// <inheritdoc />
    public partial class CreateOrderItems4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "OrderItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "OrderItems");
        }
    }
}
