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
               columns: new[] { "Id", "ClubName", "AverageMaxCapacity", "ChallengeRewardsCountForPeriod", "PeriodOfRewardReset", "ResetDayForRewards", "ChallengeInitiationDelayHours", "ReservationHours", "BonusTimeAfterReservationExpiration", "PhoneNumber", "DaysOff", "StartWorkingHours", "EndWorkingHours", "IsCustomPeriodOn", "IsCustomPeriodSetupComplete" },
               values: new object[] { 1, "Club", 100, 5, "Weekly", "Sunday", 6, "15:30, 15:00, 12:30", 10, "088-88-88-88", "Monday", "20:00", "20:00", false, false }
           );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
