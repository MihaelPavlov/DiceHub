using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260729145000_FixSeedGameCatalogCategories")]
    public partial class FixSeedGameCatalogCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "SeedGameCatalog"
                SET "CategoryName" = categories."CategoryName"
                FROM (VALUES
                    ('Azul: Queen''s Garden', 'Abstract'),
                    ('Citadel of Time', 'Strategy'),
                    ('Dixit: Stella', 'Party'),
                    ('UNO Flip', 'Card'),
                    ('UNO', 'Card'),
                    ('MicroMacro: Showdown', 'Cooperative'),
                    ('Exploding Kittens: Good vs Evil', 'Card'),
                    ('MicroMacro: Crime City', 'Cooperative'),
                    ('Dixit', 'Party'),
                    ('Dungeon Board Game', 'Adventure'),
                    ('Among Thieves', 'Strategy'),
                    ('Here to Slay', 'Card'),
                    ('In Too Deep', 'Cooperative'),
                    ('On a Scale of One to Trex', 'Party'),
                    ('Discover: Lands Unknown', 'Exploration'),
                    ('Comanauts', 'Adventure'),
                    ('Round the World', 'Family'),
                    ('A War of Whispers', 'Warfare'),
                    ('The Arrival', 'Exploration'),
                    ('Master F Orion', 'Science Fiction'),
                    ('Skyward', 'Resource Management'),
                    ('Exploding Kittens', 'Card'),
                    ('7 Wonders Duel', 'Strategy'),
                    ('Battalia: The Creation', 'Warfare')
                ) AS categories("Name", "CategoryName")
                WHERE "SeedGameCatalog"."Name" = categories."Name";
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "SeedGameCatalog"
                SET "CategoryName" = 'Warfare';
                """);
        }
    }
}
