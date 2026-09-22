using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleksitid.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddArbeidstid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arbeidstider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Navn = table.Column<string>(type: "TEXT", nullable: false),
                    TimerPerUke = table.Column<decimal>(type: "TEXT", nullable: false),
                    FraDato = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TilDato = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arbeidstider", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arbeidstider");
        }
    }
}
