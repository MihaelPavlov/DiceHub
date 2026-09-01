using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    // Hand-written (this project's migrations are). 20260901120000_AddQrTokens
    // created the table + RLS policy but not the table grant for the runtime
    // role, and it has already been applied on environments that took the
    // earlier backend builds - so the grant ships as its own idempotent
    // migration. GRANT is repeatable; safe to run wherever it lands.
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260901130000_GrantQrTokensToAppUser")]
    public partial class GrantQrTokensToAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'dicehub_user')
                       AND to_regclass('public.""QrTokens""') IS NOT NULL THEN
                        GRANT SELECT, INSERT, UPDATE, DELETE ON public.""QrTokens"" TO dicehub_user;
                        GRANT USAGE, SELECT ON SEQUENCE public.""QrTokens_Id_seq"" TO dicehub_user;
                    END IF;
                END $$;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'dicehub_user')
                       AND to_regclass('public.""QrTokens""') IS NOT NULL THEN
                        REVOKE SELECT, INSERT, UPDATE, DELETE ON public.""QrTokens"" FROM dicehub_user;
                        REVOKE USAGE, SELECT ON SEQUENCE public.""QrTokens_Id_seq"" FROM dicehub_user;
                    END IF;
                END $$;
            ");
        }
    }
}
