using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomPeriodEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomPeriodChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodChallenges_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomPeriodRewards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequiredPoints = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RewardId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodRewards_ChallengeRewards_RewardId",
                        column: x => x.RewardId,
                        principalTable: "ChallengeRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomPeriodUserChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AttemptCount = table.Column<int>(type: "integer", nullable: false),
                    IsRewardCollected = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CustomPeriodChallengeId = table.Column<int>(type: "integer", nullable: false),
                    UserChallengePeriodPerformanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodUserChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserChallenges_CustomPeriodChallenges_CustomPer~",
                        column: x => x.CustomPeriodChallengeId,
                        principalTable: "CustomPeriodChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserChallenges_UserChallengePeriodPerformances_~",
                        column: x => x.UserChallengePeriodPerformanceId,
                        principalTable: "UserChallengePeriodPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomPeriodUserRewards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IsClaimed = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CustomPeriodRewardId = table.Column<int>(type: "integer", nullable: false),
                    UserChallengePeriodPerformanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodUserRewards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserRewards_CustomPeriodRewards_CustomPeriodRew~",
                        column: x => x.CustomPeriodRewardId,
                        principalTable: "CustomPeriodRewards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserRewards_UserChallengePeriodPerformances_Use~",
                        column: x => x.UserChallengePeriodPerformanceId,
                        principalTable: "UserChallengePeriodPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodChallenges_GameId",
                table: "CustomPeriodChallenges",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodRewards_RewardId",
                table: "CustomPeriodRewards",
                column: "RewardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserChallenges_CustomPeriodChallengeId",
                table: "CustomPeriodUserChallenges",
                column: "CustomPeriodChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserChallenges_UserChallengePeriodPerformanceId",
                table: "CustomPeriodUserChallenges",
                column: "UserChallengePeriodPerformanceId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserRewards_CustomPeriodRewardId",
                table: "CustomPeriodUserRewards",
                column: "CustomPeriodRewardId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserRewards_UserChallengePeriodPerformanceId",
                table: "CustomPeriodUserRewards",
                column: "UserChallengePeriodPerformanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomPeriodUserChallenges");

            migrationBuilder.DropTable(
                name: "CustomPeriodUserRewards");

            migrationBuilder.DropTable(
                name: "CustomPeriodChallenges");

            migrationBuilder.DropTable(
                name: "CustomPeriodRewards");
        }
    }
}
