using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class UniversalChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_Challenges_ChallengeId",
                table: "UserChallenges");

            migrationBuilder.AlterColumn<int>(
                name: "ChallengeId",
                table: "UserChallenges",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "UserChallenges",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniversalChallengeId",
                table: "UserChallenges",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UniversalChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false),
                    Name_EN = table.Column<string>(type: "text", nullable: false),
                    Name_BG = table.Column<string>(type: "text", nullable: false),
                    Description_EN = table.Column<string>(type: "text", nullable: false),
                    Description_BG = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    MinValue = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversalChallenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomPeriodUniversalChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Attempts = table.Column<int>(type: "integer", nullable: false),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    MinValue = table.Column<decimal>(type: "numeric", nullable: true),
                    UniversalChallengeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodUniversalChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUniversalChallenges_UniversalChallenges_Univers~",
                        column: x => x.UniversalChallengeId,
                        principalTable: "UniversalChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomPeriodUserUniversalChallenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChallengeAttempts = table.Column<int>(type: "integer", nullable: false),
                    UserAttempts = table.Column<int>(type: "integer", nullable: false),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    IsRewardCollected = table.Column<bool>(type: "boolean", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UniversalChallengeId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: true),
                    UserChallengePeriodPerformanceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomPeriodUserUniversalChallenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserUniversalChallenges_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserUniversalChallenges_UniversalChallenges_Uni~",
                        column: x => x.UniversalChallengeId,
                        principalTable: "UniversalChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomPeriodUserUniversalChallenges_UserChallengePeriodPerf~",
                        column: x => x.UserChallengePeriodPerformanceId,
                        principalTable: "UserChallengePeriodPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_GameId",
                table: "UserChallenges",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChallenges_UniversalChallengeId",
                table: "UserChallenges",
                column: "UniversalChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUniversalChallenges_UniversalChallengeId",
                table: "CustomPeriodUniversalChallenges",
                column: "UniversalChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserUniversalChallenges_GameId",
                table: "CustomPeriodUserUniversalChallenges",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserUniversalChallenges_UniversalChallengeId",
                table: "CustomPeriodUserUniversalChallenges",
                column: "UniversalChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomPeriodUserUniversalChallenges_UserChallengePeriodPerf~",
                table: "CustomPeriodUserUniversalChallenges",
                column: "UserChallengePeriodPerformanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_Challenges_ChallengeId",
                table: "UserChallenges",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_Games_GameId",
                table: "UserChallenges",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_UniversalChallenges_UniversalChallengeId",
                table: "UserChallenges",
                column: "UniversalChallengeId",
                principalTable: "UniversalChallenges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_Challenges_ChallengeId",
                table: "UserChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_Games_GameId",
                table: "UserChallenges");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChallenges_UniversalChallenges_UniversalChallengeId",
                table: "UserChallenges");

            migrationBuilder.DropTable(
                name: "CustomPeriodUniversalChallenges");

            migrationBuilder.DropTable(
                name: "CustomPeriodUserUniversalChallenges");

            migrationBuilder.DropTable(
                name: "UniversalChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_GameId",
                table: "UserChallenges");

            migrationBuilder.DropIndex(
                name: "IX_UserChallenges_UniversalChallengeId",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "UserChallenges");

            migrationBuilder.DropColumn(
                name: "UniversalChallengeId",
                table: "UserChallenges");

            migrationBuilder.AlterColumn<int>(
                name: "ChallengeId",
                table: "UserChallenges",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChallenges_Challenges_ChallengeId",
                table: "UserChallenges",
                column: "ChallengeId",
                principalTable: "Challenges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
