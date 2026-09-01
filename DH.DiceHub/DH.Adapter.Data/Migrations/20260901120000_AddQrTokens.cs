using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    // Hand-written (this project's migrations are, see 20260827130000). Adds the
    // QrTokens table that backs the short opaque QR codes, plus the same
    // tenant-isolation RLS every other TenantId table gets from
    // TenantDbUserScript.txt - that script isn't re-run automatically, and the
    // app connects as dicehub_user (no BYPASSRLS), so the policy must ship here.
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260901120000_AddQrTokens")]
    public partial class AddQrTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QrTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    EntityId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConsumedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QrTokens_Token",
                table: "QrTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QrTokens_Type_EntityId_UserId",
                table: "QrTokens",
                columns: new[] { "Type", "EntityId", "UserId" });

            migrationBuilder.Sql(@"
                ALTER TABLE public.""QrTokens"" ENABLE ROW LEVEL SECURITY;
                ALTER TABLE public.""QrTokens"" FORCE ROW LEVEL SECURITY;
                DROP POLICY IF EXISTS tenant_isolation_policy ON public.""QrTokens"";
                CREATE POLICY tenant_isolation_policy ON public.""QrTokens""
                    FOR ALL
                    USING (""TenantId"" = current_setting('app.tenant_id', true))
                    WITH CHECK (""TenantId"" = current_setting('app.tenant_id', true));
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QrTokens");
        }
    }
}
