using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Statistics.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventAttendancyLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventAttendanceLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventAttendanceLogs");
        }
    }
}
