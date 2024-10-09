using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserChallengePeriodReward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserChallengePeriodReward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeRewardId = table.Column<int>(type: "int", nullable: false),
                    UserChallengePeriodPerformanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChallengePeriodReward", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChallengePeriodReward_ChallengeRewards_ChallengeRewardId",
                        column: x => x.ChallengeRewardId,
                        principalTable: "ChallengeRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChallengePeriodReward_UserChallengePeriodPerformances_UserChallengePeriodPerformanceId",
                        column: x => x.UserChallengePeriodPerformanceId,
                        principalTable: "UserChallengePeriodPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengePeriodReward_ChallengeRewardId",
                table: "UserChallengePeriodReward",
                column: "ChallengeRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallengePeriodReward_UserChallengePeriodPerformanceId",
                table: "UserChallengePeriodReward",
                column: "UserChallengePeriodPerformanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserChallengePeriodReward");
        }
    }
}
