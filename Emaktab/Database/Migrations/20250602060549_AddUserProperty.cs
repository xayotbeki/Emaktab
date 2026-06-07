using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Emaktab.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biography",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Pasport_number",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Pasport_seria",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pinfl",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "eng_lang",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "other_lang",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "rus_lang",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "tajribasi",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "user_level",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_location",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "user_type_name",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "uzb_lang",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pasport_number",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pasport_seria",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Pinfl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "eng_lang",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "other_lang",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "rus_lang",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "tajribasi",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "user_level",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "user_location",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "user_type_name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "uzb_lang",
                table: "Users");
        }
    }
}
