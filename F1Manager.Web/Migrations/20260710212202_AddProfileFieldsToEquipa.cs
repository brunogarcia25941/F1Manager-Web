using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Manager.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileFieldsToEquipa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnoFundacao",
                table: "Equipas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChefeEquipa",
                table: "Equipas",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Historia",
                table: "Equipas",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Logotipo",
                table: "Equipas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnoFundacao",
                table: "Equipas");

            migrationBuilder.DropColumn(
                name: "ChefeEquipa",
                table: "Equipas");

            migrationBuilder.DropColumn(
                name: "Historia",
                table: "Equipas");

            migrationBuilder.DropColumn(
                name: "Logotipo",
                table: "Equipas");
        }
    }
}
