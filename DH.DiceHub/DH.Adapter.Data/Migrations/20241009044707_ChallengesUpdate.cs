using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChallengesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallengePeriodReward_ChallengeRewards_ChallengeRewardId",
                table: "UserChallengePeriodReward");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChallengePeriodReward_UserChallengePeriodPerformances_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodReward");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChallengePeriodReward",
                table: "UserChallengePeriodReward");

            migrationBuilder.RenameTable(
                name: "UserChallengePeriodReward",
                newName: "UserChallengePeriodRewards");

            migrationBuilder.RenameIndex(
                name: "IX_UserChallengePeriodReward_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodRewards",
                newName: "IX_UserChallengePeriodRewards_UserChallengePeriodPerformanceId");

            migrationBuilder.RenameIndex(
                name: "IX_UserChallengePeriodReward_ChallengeRewardId",
                table: "UserChallengePeriodRewards",
                newName: "IX_UserChallengePeriodRewards_ChallengeRewardId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresDate",
                table: "UserChallengeRewards",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClaimedDate",
                table: "UserChallengeRewards",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsExpired",
                table: "UserChallengeRewards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "UserChallengePeriodRewards",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChallengePeriodRewards",
                table: "UserChallengePeriodRewards",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallengePeriodRewards_ChallengeRewards_ChallengeRewardId",
                table: "UserChallengePeriodRewards",
                column: "ChallengeRewardId",
                principalTable: "ChallengeRewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallengePeriodRewards_UserChallengePeriodPerformances_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodRewards",
                column: "UserChallengePeriodPerformanceId",
                principalTable: "UserChallengePeriodPerformances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallengePeriodRewards_ChallengeRewards_ChallengeRewardId",
                table: "UserChallengePeriodRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChallengePeriodRewards_UserChallengePeriodPerformances_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodRewards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChallengePeriodRewards",
                table: "UserChallengePeriodRewards");

            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "UserChallengeRewards");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "UserChallengePeriodRewards");

            migrationBuilder.RenameTable(
                name: "UserChallengePeriodRewards",
                newName: "UserChallengePeriodReward");

            migrationBuilder.RenameIndex(
                name: "IX_UserChallengePeriodRewards_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodReward",
                newName: "IX_UserChallengePeriodReward_UserChallengePeriodPerformanceId");

            migrationBuilder.RenameIndex(
                name: "IX_UserChallengePeriodRewards_ChallengeRewardId",
                table: "UserChallengePeriodReward",
                newName: "IX_UserChallengePeriodReward_ChallengeRewardId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresDate",
                table: "UserChallengeRewards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClaimedDate",
                table: "UserChallengeRewards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChallengePeriodReward",
                table: "UserChallengePeriodReward",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallengePeriodReward_ChallengeRewards_ChallengeRewardId",
                table: "UserChallengePeriodReward",
                column: "ChallengeRewardId",
                principalTable: "ChallengeRewards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallengePeriodReward_UserChallengePeriodPerformances_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodReward",
                column: "UserChallengePeriodPerformanceId",
                principalTable: "UserChallengePeriodPerformances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
