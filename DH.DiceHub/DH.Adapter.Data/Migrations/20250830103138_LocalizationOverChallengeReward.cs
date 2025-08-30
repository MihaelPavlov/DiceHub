using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class LocalizationOverChallengeReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ChallengeRewards",
                newName: "Name_EN");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ChallengeRewards",
                newName: "Name_BG");

            migrationBuilder.AddColumn<string>(
                name: "Description_BG",
                table: "ChallengeRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description_EN",
                table: "ChallengeRewards",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description_BG",
                table: "ChallengeRewards");

            migrationBuilder.DropColumn(
                name: "Description_EN",
                table: "ChallengeRewards");

            migrationBuilder.RenameColumn(
                name: "Name_EN",
                table: "ChallengeRewards",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Name_BG",
                table: "ChallengeRewards",
                newName: "Description");
        }
    }
}
