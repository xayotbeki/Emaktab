using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Emaktab.Migrations
{
    /// <inheritdoc />
    public partial class AddIshRejaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IshRejalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Nomi = table.Column<string>(type: "text", nullable: false),
                    FanNomi = table.Column<string>(type: "text", nullable: true),
                    Guruh = table.Column<string>(type: "text", nullable: true),
                    Tavsif = table.Column<string>(type: "text", nullable: true),
                    Sana = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IshRejalar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IshRejalar");
        }
    }
}
