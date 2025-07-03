using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomPeriodModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomPeriodUserChallenges_CustomPeriodChallenges_CustomPer~",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomPeriodUserRewards_CustomPeriodRewards_CustomPeriodRew~",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropIndex(
                name: "IX_CustomPeriodUserRewards_CustomPeriodRewardId",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropIndex(
                name: "IX_CustomPeriodUserChallenges_CustomPeriodChallengeId",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomPeriodRewards");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomPeriodRewards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomPeriodChallenges");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomPeriodChallenges");

            migrationBuilder.RenameColumn(
                name: "CustomPeriodRewardId",
                table: "CustomPeriodUserRewards",
                newName: "RequiredPoints");

            migrationBuilder.RenameColumn(
                name: "CustomPeriodChallengeId",
                table: "CustomPeriodUserChallenges",
                newName: "RewardPoints");

            migrationBuilder.RenameColumn(
                name: "AttemptCount",
                table: "CustomPeriodUserChallenges",
                newName: "Attempts");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "CustomPeriodUserChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.RenameColumn(
                name: "RequiredPoints",
                table: "CustomPeriodUserRewards",
                newName: "CustomPeriodRewardId");

            migrationBuilder.RenameColumn(
                name: "RewardPoints",
                table: "CustomPeriodUserChallenges",
                newName: "CustomPeriodChallengeId");

            migrationBuilder.RenameColumn(
                name: "Attempts",
                table: "CustomPeriodUserChallenges",
                newName: "AttemptCount");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomPeriodRewards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomPeriodRewards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomPeriodChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomPeriodChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserRewards_CustomPeriodRewardId",
                table: "CustomPeriodUserRewards",
                column: "CustomPeriodRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserChallenges_CustomPeriodChallengeId",
                table: "CustomPeriodUserChallenges",
                column: "CustomPeriodChallengeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomPeriodUserChallenges_CustomPeriodChallenges_CustomPer~",
                table: "CustomPeriodUserChallenges",
                column: "CustomPeriodChallengeId",
                principalTable: "CustomPeriodChallenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomPeriodUserRewards_CustomPeriodRewards_CustomPeriodRew~",
                table: "CustomPeriodUserRewards",
                column: "CustomPeriodRewardId",
                principalTable: "CustomPeriodRewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
