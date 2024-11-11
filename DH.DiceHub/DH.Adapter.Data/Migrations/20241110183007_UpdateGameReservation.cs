using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsExpired",
                table: "GameReservations",
                newName: "IsReservationSuccessful");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "GameReservations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PeopleCount",
                table: "GameReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "GameReservations");

            migrationBuilder.DropColumn(
                name: "PeopleCount",
                table: "GameReservations");

            migrationBuilder.RenameColumn(
                name: "IsReservationSuccessful",
                table: "GameReservations",
                newName: "IsExpired");
        }
    }
}
