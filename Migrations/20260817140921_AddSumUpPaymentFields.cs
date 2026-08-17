using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NutShop.Migrations
{
    /// <inheritdoc />
    public partial class AddSumUpPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SumUpCheckoutId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SumUpCheckoutReference",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SumUpTransactionCode",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SumUpCheckoutId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SumUpCheckoutReference",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SumUpTransactionCode",
                table: "Orders");
        }
    }
}
