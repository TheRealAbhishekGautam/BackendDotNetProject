using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Razor_Project.Migrations
{
    /// <inheritdoc />
    public partial class MigraitonOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catagory",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catagory", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Catagory",
                columns: new[] { "ID", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "SciFi" },
                    { 2, 2, "Drama" },
                    { 3, 3, "Funky" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Catagory");
        }
    }
}
