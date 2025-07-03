using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOverCustomPeriodEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.RenameColumn(
                name: "IsClaimed",
                table: "CustomPeriodUserRewards",
                newName: "IsCompleted");

            migrationBuilder.RenameColumn(
                name: "Attempts",
                table: "CustomPeriodUserChallenges",
                newName: "UserAttempts");

            migrationBuilder.AddColumn<int>(
                name: "RewardId",
                table: "CustomPeriodUserRewards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChallengeAttempts",
                table: "CustomPeriodUserChallenges",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "CustomPeriodUserChallenges",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "CustomPeriodUserChallenges",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserRewards_RewardId",
                table: "CustomPeriodUserRewards",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserChallenges_GameId",
                table: "CustomPeriodUserChallenges",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomPeriodUserChallenges_Games_GameId",
                table: "CustomPeriodUserChallenges",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomPeriodUserRewards_ChallengeRewards_RewardId",
                table: "CustomPeriodUserRewards",
                column: "RewardId",
                principalTable: "ChallengeRewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomPeriodUserChallenges_Games_GameId",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomPeriodUserRewards_ChallengeRewards_RewardId",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropIndex(
                name: "IX_CustomPeriodUserRewards_RewardId",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropIndex(
                name: "IX_CustomPeriodUserChallenges_GameId",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropColumn(
                name: "RewardId",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropColumn(
                name: "ChallengeAttempts",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "CustomPeriodUserRewards",
                newName: "IsClaimed");

            migrationBuilder.RenameColumn(
                name: "UserAttempts",
                table: "CustomPeriodUserChallenges",
                newName: "Attempts");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomPeriodUserRewards",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomPeriodUserChallenges",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
