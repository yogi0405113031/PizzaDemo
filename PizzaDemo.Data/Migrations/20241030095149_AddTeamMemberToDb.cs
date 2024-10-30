using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PizzaDemo.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamMemberToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Introduction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TeamMembers",
                columns: new[] { "Id", "ImageUrl", "Introduction", "Name", "Position" },
                values: new object[,]
                {
                    { 1, "", "對披薩烹飪充滿熱情，樂於嘗試新事物、接受顧客的建議並持續改進。他認為好的披薩應該不僅滿足顧客的味蕾，更能帶給顧客幸福感。", "王曉明", "主廚" },
                    { 2, "", "熱愛披薩料理，致力於創新和優化口味。他在工作中注重細節並保持積極學習的態度，並相信每一款披薩都應該展現出料理人的用心和對品質的追求。", "陳大鵬", "副主廚" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMembers");
        }
    }
}
