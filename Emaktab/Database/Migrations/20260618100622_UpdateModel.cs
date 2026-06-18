using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emaktab.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Documents_user_id",
                table: "Documents",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_create_userId",
                table: "Blogs",
                column: "create_userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_create_userId",
                table: "Blogs",
                column: "create_userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_user_id",
                table: "Documents",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_create_userId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_user_id",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_user_id",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_create_userId",
                table: "Blogs");
        }
    }
}
