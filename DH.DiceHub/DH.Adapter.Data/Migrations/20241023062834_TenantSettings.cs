using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class TenantSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AverageMaxCapacity = table.Column<int>(type: "int", nullable: false),
                    ChallengeRewardsCountForPeriod = table.Column<int>(type: "int", nullable: false),
                    PeriodOfRewardReset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResetDayForRewards = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChallengeInitiationDelayHours = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TenantSettings",
                columns: new[] { "Id", "AverageMaxCapacity", "ChallengeRewardsCountForPeriod", "PeriodOfRewardReset", "ResetDayForRewards", "ChallengeInitiationDelayHours" },
                values: new object[] { 1, 100, 5, "Weekly", "Sunday", 6 }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantSettings");
        }
    }
}
