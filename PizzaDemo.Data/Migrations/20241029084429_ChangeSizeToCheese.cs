using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaDemo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSizeToCheese : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Size",
                table: "shoppingCarts",
                newName: "Cheese");

            migrationBuilder.RenameColumn(
                name: "Size",
                table: "OrderDetails",
                newName: "Cheese");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cheese",
                table: "shoppingCarts",
                newName: "Size");

            migrationBuilder.RenameColumn(
                name: "Cheese",
                table: "OrderDetails",
                newName: "Size");
        }
    }
}
