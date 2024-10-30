using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaDemo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMemberImageUrlToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "\\images\\product\\80f2b999-1a13-4915-92be-f03e44c7ff83.png");

            migrationBuilder.UpdateData(
                table: "TeamMembers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/team/21b2d8dd-7877-4c4e-81ea-51a4673b7110.jpg");

            migrationBuilder.UpdateData(
                table: "TeamMembers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/team/c778129f-12f4-4646-a127-22bf43137f15.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "\\images\\product\\2376d0c5-785c-475c-9bdf-526670bb6640.png");

            migrationBuilder.UpdateData(
                table: "TeamMembers",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "TeamMembers",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "");
        }
    }
}
