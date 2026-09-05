using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueCustomPeriodUniversalChallengeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Unique_CustomPeriodUniversalChallenge_Per_Tenant",
                table: "CustomPeriodUniversalChallenges",
                columns: new[] { "TenantId", "UniversalChallengeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unique_CustomPeriodUniversalChallenge_Per_Tenant",
                table: "CustomPeriodUniversalChallenges");
        }
    }
}
