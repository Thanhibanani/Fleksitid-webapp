using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fleksitid.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClockTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "TimeEntries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "TimeEntries",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TimeEntries");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TimeEntries");
        }
    }
}
