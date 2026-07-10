using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Manager.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFieldsToPiloto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biografia",
                table: "Pilotos",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfil",
                table: "Pilotos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "Peso",
                table: "Pilotos",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biografia",
                table: "Pilotos");

            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Pilotos");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Pilotos");
        }
    }
}
