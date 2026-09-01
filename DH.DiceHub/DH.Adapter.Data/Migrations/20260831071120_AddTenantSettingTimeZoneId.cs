using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantSettingTimeZoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // NOTE: `dotnet ef migrations add` also emitted Tenants.TenantApplicationId
            // (+ index + FK) here because 20260731120000_AddTenantApplicationLink was
            // hand-written and never updated the model snapshot. That column already
            // exists in every environment that ran AddTenantApplicationLink, so adding
            // it again would fail. Those operations were removed from this migration;
            // the snapshot now carries them so future migrations stay clean.
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "TenantSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "TenantSettings");
        }
    }
}
