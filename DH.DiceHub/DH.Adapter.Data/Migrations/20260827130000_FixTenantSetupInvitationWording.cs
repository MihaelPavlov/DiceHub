using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    // Data-only fix: the seeded English TenantSetupInvitation email said "start
    // setting up your tenant" / "Start tenant setup" - wording a venue owner
    // applicant has no reason to understand. The app-level seeder never updates
    // an EmailTemplate row that already exists, so this already-provisioned
    // production content needed a real migration, not just a code change.
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260827130000_FixTenantSetupInvitationWording")]
    public partial class FixTenantSetupInvitationWording : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "EmailTemplates"
                SET "TemplateHtml" = REPLACE(
                    REPLACE("TemplateHtml", 'start setting up your tenant:', 'start setting up your venue:'),
                    'Start tenant setup</a>', 'Start venue setup</a>'
                )
                WHERE "TemplateName" = 'TenantSetupInvitation' AND "Language" = 'EN';
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "EmailTemplates"
                SET "TemplateHtml" = REPLACE(
                    REPLACE("TemplateHtml", 'start setting up your venue:', 'start setting up your tenant:'),
                    'Start venue setup</a>', 'Start tenant setup</a>'
                )
                WHERE "TemplateName" = 'TenantSetupInvitation' AND "Language" = 'EN';
                """);
        }
    }
}
