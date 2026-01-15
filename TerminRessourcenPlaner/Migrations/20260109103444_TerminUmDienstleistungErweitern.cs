using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TerminRessourcenPlaner.Migrations
{
    /// <inheritdoc />
    public partial class TerminUmDienstleistungErweitern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DienstleistungId",
                table: "Termine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Kundenname",
                table: "Termine",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Dienstleistung",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Bezeichnung = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DauerInMinuten = table.Column<int>(type: "int", nullable: false),
                    PreisInEuro = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Kategorie = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dienstleistung", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Termine_DienstleistungId",
                table: "Termine",
                column: "DienstleistungId");

            migrationBuilder.AddForeignKey(
                name: "FK_Termine_Dienstleistung_DienstleistungId",
                table: "Termine",
                column: "DienstleistungId",
                principalTable: "Dienstleistung",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Termine_Dienstleistung_DienstleistungId",
                table: "Termine");

            migrationBuilder.DropTable(
                name: "Dienstleistung");

            migrationBuilder.DropIndex(
                name: "IX_Termine_DienstleistungId",
                table: "Termine");

            migrationBuilder.DropColumn(
                name: "DienstleistungId",
                table: "Termine");

            migrationBuilder.DropColumn(
                name: "Kundenname",
                table: "Termine");
        }
    }
}
