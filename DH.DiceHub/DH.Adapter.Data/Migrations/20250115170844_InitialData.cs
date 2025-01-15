using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "TenantSettings",
               columns: new[] { "Id", "AverageMaxCapacity", "ChallengeRewardsCountForPeriod", "PeriodOfRewardReset", "ResetDayForRewards", "ChallengeInitiationDelayHours", "ReservationHours", "BonusTimeAfterReservationExpiration", "PhoneNumber" },
               values: new object[] { 1, 100, 5, "Weekly", "Sunday", 6, "15:30, 15:00, 12:30", 10, "088-88-88-88"}
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
