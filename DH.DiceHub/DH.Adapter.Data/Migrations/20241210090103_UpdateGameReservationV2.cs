using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameReservationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PeopleCount",
                table: "GameReservations",
                newName: "NumberOfGuests");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "GameReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "GameReservations");

            migrationBuilder.RenameColumn(
                name: "NumberOfGuests",
                table: "GameReservations",
                newName: "PeopleCount");
        }
    }
}
