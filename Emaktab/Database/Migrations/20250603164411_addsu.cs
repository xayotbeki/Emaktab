using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emaktab.Migrations
{
    /// <inheritdoc />
    public partial class addsu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "create_userId",
                table: "Blogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "create_userId",
                table: "Blogs");
        }
    }
}
