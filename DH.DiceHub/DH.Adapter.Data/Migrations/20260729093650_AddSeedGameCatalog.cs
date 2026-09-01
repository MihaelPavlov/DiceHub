using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DH.Adapter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedGameCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeedGameCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description_EN = table.Column<string>(type: "text", nullable: false),
                    Description_BG = table.Column<string>(type: "text", nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MinPlayers = table.Column<int>(type: "integer", nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    AveragePlaytime = table.Column<int>(type: "integer", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: false),
                    ImageFileName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeedGameCatalog", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SeedGameCatalog",
                columns:
                [
                    "Id",
                    "Name",
                    "Description_EN",
                    "Description_BG",
                    "MinAge",
                    "MinPlayers",
                    "MaxPlayers",
                    "AveragePlaytime",
                    "CategoryName",
                    "ImageUrl",
                    "ImageFileName",
                    "IsActive",
                    "CreatedDate"
                ],
                values: new object[,]
                {
                    { 1, "Azul: Queen's Garden", "Azul: Queen's Garden is a strategic tile-laying game where players decorate the queen's garden. Players take turns drafting tiles and placing them on their board to score points.", "Azul: Queen's Garden е стратегическа игра с плочки, където играчите декорират градината на кралицата. Играчите вземат плочки на ред и ги поставят на своята дъска, за да печелят точки.", 8, 2, 4, 30, "Abstract", string.Empty, "azul_queens_garden.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 2, "Citadel of Time", "Citadel of Time is a cooperative board game where players work together to defend the citadel from invading forces, using time-based mechanics to strategize their moves.", "Citadel of Time е кооперативна настолна игра, в която играчите работят заедно, за да защитят цитаделата от нашественици, използвайки механики, базирани на времето, за стратегическо планиране на ходовете.", 10, 2, 4, 30, "Strategy", string.Empty, "citadel_of_time.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 3, "Dixit: Stella", "Dixit: Stella is a storytelling card game with imaginative illustrations and creative clues.", "Dixit: Stella е игра с разказване на истории с картите, с въображаеми илюстрации и креативни улики.", 8, 3, 6, 30, "Party", string.Empty, "dixit_stella.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 4, "UNO Flip", "UNO Flip is a variation of the classic UNO card game with a double-sided deck and new rules.", "UNO Flip е вариант на класическата игра UNO с двустранно тесте и нови правила.", 7, 2, 10, 20, "Card", string.Empty, "uno_flip.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 5, "UNO", "UNO is the classic card game where players match colors and numbers to be the first to play all their cards.", "UNO е класическата игра с карти, където играчите съчетават цветове и числа, за да изиграят първи всичките си карти.", 7, 2, 10, 20, "Card", string.Empty, "uno.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 6, "MicroMacro: Showdown", "MicroMacro: Showdown is a cooperative detective game on a large city map, finding crimes and solving mysteries.", "MicroMacro: Showdown е кооперативна детективска игра на голяма карта на града, откриване на престъпления и решаване на мистерии.", 8, 1, 4, 30, "Cooperative", string.Empty, "micro_macro_showdown.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 7, "Exploding Kittens: Good vs Evil", "Exploding Kittens: Good vs Evil is a fast-paced card game full of strategic risk-taking and explosive fun.", "Exploding Kittens: Good vs Evil е бърза игра с карти с стратегически риск и експлозивно забавление.", 7, 2, 5, 15, "Card", string.Empty, "exploding_kittens_good_vs_evil.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 8, "MicroMacro: Crime City", "MicroMacro: Crime City challenges players to investigate a city map to solve hidden crimes.", "MicroMacro: Crime City предизвиква играчите да разследват карта на града, за да решат скрити престъпления.", 8, 1, 4, 30, "Cooperative", string.Empty, "micro_macro_crime_city.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 9, "Dixit", "Dixit is a storytelling card game with beautiful illustrations where players guess each other's cards.", "Dixit е игра с разказване на истории с красиви илюстрации, където играчите отгатват картите на другите.", 8, 3, 6, 30, "Party", string.Empty, "dixit.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 10, "Dungeon Board Game", "Dungeon Board Game is a dungeon-crawling adventure game with heroes, monsters, and treasures.", "Dungeon Board Game е приключенска игра с герои, чудовища и съкровища, изследващи подземия.", 10, 2, 5, 60, "Adventure", string.Empty, "dungeon_board_game.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 11, "Among Thieves", "Among Thieves is a strategic heist game where players plan and execute robberies.", "Among Thieves е стратегическа игра за обири, където играчите планират и изпълняват кражби.", 10, 2, 4, 45, "Strategy", string.Empty, "among_thieves.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 12, "Here to Slay", "Here to Slay is a strategic card game where players recruit heroes, battle monsters, and slay opponents to win.", "Here to Slay е стратегическа игра с карти, където играчите набират герои, сражават се с чудовища и побеждават противници.", 14, 2, 6, 30, "Card", string.Empty, "here_to_slay.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 13, "In Too Deep", "In Too Deep is a cooperative card game about underwater exploration and escaping hazards.", "In Too Deep е кооперативна игра с карти за подводно изследване и избягване на опасности.", 10, 2, 5, 30, "Cooperative", string.Empty, "in_too_deep.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 14, "On a Scale of One to Trex", "On a Scale of One to Trex is a family-friendly party game where players rate outrageous scenarios.", "On a Scale of One to Trex е забавна семейна игра, в която играчите оценяват невероятни сценарии.", 10, 3, 8, 20, "Party", string.Empty, "on_a_scale_of_one_to_trex.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 15, "Discover: Lands Unknown", "Discover: Lands Unknown is an exploration board game where players uncover and colonize new lands.", "Discover: Lands Unknown е изследователска настолна игра, в която играчите откриват и колонизират нови земи.", 12, 2, 4, 60, "Exploration", string.Empty, "discover_lands_unkown.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 16, "Comanauts", "Comanauts is a light strategy game where players compete to explore space and gather resources.", "Comanauts е стратегическа игра, в която играчите изследват космоса и събират ресурси.", 10, 2, 4, 30, "Adventure", string.Empty, "comanauts.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 17, "Round the World", "Round the World is a travel-themed board game where players race to visit continents and collect points.", "Round the World е настолна игра с тема пътувания, в която играчите се състезават да посетят континенти и да събират точки.", 8, 2, 6, 45, "Family", string.Empty, "round_the_word.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 18, "A War of Whispers", "A War of Whispers is a secret influence board game where players manipulate leaders to gain power.", "A War of Whispers е настолна игра със скрита манипулация, където играчите влияят на лидери, за да спечелят власт.", 12, 2, 4, 45, "Warfare", string.Empty, "a_war_of_whispers.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 19, "The Arrival", "The Arrival is an adventure board game where players navigate new lands and encounter challenges.", "The Arrival е приключенска настолна игра, в която играчите изследват нови земи и срещат предизвикателства.", 10, 2, 4, 60, "Exploration", string.Empty, "the_arrival.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 20, "Master F Orion", "Master F Orion is a space strategy board game with exploration, trade, and combat mechanics.", "Master F Orion е стратегическа настолна игра в космоса с изследване, търговия и бойни механики.", 12, 2, 4, 60, "Science Fiction", string.Empty, "master_f_orion.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 21, "Skyward", "Skyward is a tactical board game where players manage resources and control territories in the skies.", "Skyward е тактическа настолна игра, в която играчите управляват ресурси и контролират територии в небето.", 10, 2, 4, 45, "Resource Management", string.Empty, "skyward.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 22, "Exploding Kittens", "Exploding Kittens is a light, fast-paced card game full of strategy and explosive fun.", "Exploding Kittens е лека, бърза игра с карти, пълна със стратегия и експлозивно забавление.", 7, 2, 5, 15, "Card", string.Empty, "exploding_kittens.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 23, "7 Wonders Duel", "7 Wonders Duel is a two-player card game where players build civilizations and compete for supremacy.", "7 Wonders Duel е двама играчи игра с карти, в която играчите изграждат цивилизации и се състезават за превъзходство.", 10, 2, 2, 30, "Strategy", string.Empty, "7_wonders_duel.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                    { 24, "Battalia: The Creation", "Battalia: The Creation is a strategy board game where players control armies and compete for dominance.", "Battalia: The Creation е стратегическа настолна игра, в която играчите управляват армии и се състезават за превъзходство.", 12, 2, 4, 60, "Warfare", string.Empty, "battalia_the_creation.jpg", true, new DateTime(2026, 7, 29, 9, 36, 50, DateTimeKind.Utc) },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeedGameCatalog");
        }
    }
}
