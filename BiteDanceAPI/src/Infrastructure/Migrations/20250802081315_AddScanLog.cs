using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiteDanceAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddScanLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScanLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ShiftOrderId = table.Column<int>(type: "int", nullable: false),
                    ScanTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScanCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Log = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScanLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScanLogs");
        }
    }
}
