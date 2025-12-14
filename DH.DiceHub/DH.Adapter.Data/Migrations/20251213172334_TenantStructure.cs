using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class TenantStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserNotifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserDeviceTokens",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserChallengeRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserChallengePeriodRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserChallengePeriodPerformances",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UniversalChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "TenantUserSettings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SpaceTables",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SpaceTableReservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "SpaceTableParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Rooms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RoomParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RoomMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RoomInfoMessages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "RewardHistoryLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ReservationOutcomeLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "QueuedJobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "QrCodeScanAudits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "PartnerInquiries",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameReviews",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameReservations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameLikes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameInventories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameEngagementLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "GameCategories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "FailedJobs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EventParticipants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EventNotification",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EventImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EventAttendanceLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EmailTemplates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "EmailHistory",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodUserUniversalChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodUserRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodUserChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodUniversalChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "CustomPeriodChallenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ClubVisitorLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ChallengeStatistics",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Challenges",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ChallengeRewards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ChallengeRewardImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "ChallengeHistoryLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TenantName = table.Column<string>(type: "text", nullable: false),
                    Town = table.Column<string>(type: "text", nullable: false),
                    TenantStatus = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RegisterQrCode = table.Column<string>(type: "text", nullable: false),
                    TenantSettingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_TenantSettings_TenantSettingId",
                        column: x => x.TenantSettingId,
                        principalTable: "TenantSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_TenantSettingId",
                table: "Tenants",
                column: "TenantSettingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserStatistics");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserDeviceTokens");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserChallengeRewards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserChallengePeriodRewards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserChallengePeriodPerformances");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UniversalChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TenantUserSettings");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SpaceTables");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SpaceTableReservations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "SpaceTableParticipants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoomParticipants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoomMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RoomInfoMessages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RewardHistoryLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ReservationOutcomeLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "QueuedJobs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "QrCodeScanAudits");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PartnerInquiries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameReviews");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameReservations");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameLikes");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameInventories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameEngagementLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "GameCategories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "FailedJobs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EventParticipants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EventNotification");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EventImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EventAttendanceLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "EmailHistory");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodUserUniversalChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodUserRewards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodUserChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodUniversalChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodRewards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CustomPeriodChallenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ClubVisitorLogs");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ChallengeStatistics");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ChallengeRewards");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ChallengeRewardImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ChallengeHistoryLogs");
        }
    }
}
