using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations;

[DbContext(typeof(TenantDbContext))]
[Migration("20260731120000_AddTenantApplicationLink")]
public partial class AddTenantApplicationLink : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "TenantApplicationId",
            table: "Tenants",
            type: "integer",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Tenants_TenantApplicationId",
            table: "Tenants",
            column: "TenantApplicationId");

        migrationBuilder.AddForeignKey(
            name: "FK_Tenants_TenantApplications_TenantApplicationId",
            table: "Tenants",
            column: "TenantApplicationId",
            principalTable: "TenantApplications",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("FK_Tenants_TenantApplications_TenantApplicationId", "Tenants");
        migrationBuilder.DropIndex("IX_Tenants_TenantApplicationId", "Tenants");
        migrationBuilder.DropColumn("TenantApplicationId", "Tenants");
    }
}
