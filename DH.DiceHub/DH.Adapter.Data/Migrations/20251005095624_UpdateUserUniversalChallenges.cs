using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserUniversalChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "CustomPeriodUserUniversalChallenges",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "CustomPeriodUserUniversalChallenges");
        }
    }
}
