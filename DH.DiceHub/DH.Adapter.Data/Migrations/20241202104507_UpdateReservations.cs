using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "SpaceTableReservations");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SpaceTableReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GameReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SpaceTableReservations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GameReservations");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "SpaceTableReservations",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
