using DH.Domain.Adapters.FileManager;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Seeder;
using Microsoft.EntityFrameworkCore;

namespace DH.Adapter.Data.Seeder;

internal class GameSeeder(IDbContextFactory<TenantDbContext> contextFactory, IFileManagerClient fileManagerClient) : IGameSeeder
{
    readonly IDbContextFactory<TenantDbContext> contextFactory = contextFactory;
    readonly IFileManagerClient fileManagerClient = fileManagerClient;
    public async Task SeedAsync()
    {
        var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "game-images");

        var games = new List<SeedGameData>()
        {
           new SeedGameData(
                new Game
                {
                    Name = "Azul: Queen's Garden",
                    Description_EN = "Azul: Queen's Garden is a strategic tile-laying game where players decorate the queen’s garden. Players take turns drafting tiles and placing them on their board to score points.",
                    Description_BG = "Azul: Queen's Garden е стратегическа игра с плочки, където играчите декорират градината на кралицата. Играчите вземат плочки на ред и ги поставят на своята дъска, за да печелят точки.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "azul_queens_garden.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Citadel of Time",
                    Description_EN = "Citadel of Time is a cooperative board game where players work together to defend the citadel from invading forces, using time-based mechanics to strategize their moves.",
                    Description_BG = "Citadel of Time е кооперативна настолна игра, в която играчите работят заедно, за да защитят цитаделата от нашественици, използвайки механики, базирани на времето, за стратегическо планиране на ходовете.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "citadel_of_time.jpg"
            ),
             new SeedGameData(
                new Game
                {
                    Name = "Dixit: Stella",
                    Description_EN = "Dixit: Stella is a storytelling card game with imaginative illustrations and creative clues.",
                    Description_BG = "Dixit: Stella е игра с разказване на истории с картите, с въображаеми илюстрации и креативни улики.",
                    MinPlayers = 3,
                    MaxPlayers = 6,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "dixit_stella.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "UNO Flip",
                    Description_EN = "UNO Flip is a variation of the classic UNO card game with a double-sided deck and new rules.",
                    Description_BG = "UNO Flip е вариант на класическата игра UNO с двустранно тесте и нови правила.",
                    MinPlayers = 2,
                    MaxPlayers = 10,
                    MinAge = 7,
                    AveragePlaytime = GameAveragePlaytime.Twenty,
                    CategoryId = 13,
                },
                "uno_flip.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "UNO",
                    Description_EN = "UNO is the classic card game where players match colors and numbers to be the first to play all their cards.",
                    Description_BG = "UNO е класическата игра с карти, където играчите съчетават цветове и числа, за да изиграят първи всичките си карти.",
                    MinPlayers = 2,
                    MaxPlayers = 10,
                    MinAge = 7,
                    AveragePlaytime = GameAveragePlaytime.Twenty,
                    CategoryId = 13,
                },
                "uno.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "MicroMacro: Showdown",
                    Description_EN = "MicroMacro: Showdown is a cooperative detective game on a large city map, finding crimes and solving mysteries.",
                    Description_BG = "MicroMacro: Showdown е кооперативна детективска игра на голяма карта на града, откриване на престъпления и решаване на мистерии.",
                    MinPlayers = 1,
                    MaxPlayers = 4,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "micro_macro_showdown.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Exploding Kittens: Good vs Evil",
                    Description_EN = "Exploding Kittens: Good vs Evil is a fast-paced card game full of strategic risk-taking and explosive fun.",
                    Description_BG = "Exploding Kittens: Good vs Evil е бърза игра с карти с стратегически риск и експлозивно забавление.",
                    MinPlayers = 2,
                    MaxPlayers = 5,
                    MinAge = 7,
                    AveragePlaytime = GameAveragePlaytime.Fifteen,
                    CategoryId = 13,
                },
                "exploding_kittens_good_vs_evil.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "MicroMacro: Crime City",
                    Description_EN = "MicroMacro: Crime City challenges players to investigate a city map to solve hidden crimes.",
                    Description_BG = "MicroMacro: Crime City предизвиква играчите да разследват карта на града, за да решат скрити престъпления.",
                    MinPlayers = 1,
                    MaxPlayers = 4,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "micro_macro_crime_city.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Dixit",
                    Description_EN = "Dixit is a storytelling card game with beautiful illustrations where players guess each other’s cards.",
                    Description_BG = "Dixit е игра с разказване на истории с красиви илюстрации, където играчите отгатват картите на другите.",
                    MinPlayers = 3,
                    MaxPlayers = 6,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "dixit.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Dungeon Board Game",
                    Description_EN = "Dungeon Board Game is a dungeon-crawling adventure game with heroes, monsters, and treasures.",
                    Description_BG = "Dungeon Board Game е приключенска игра с герои, чудовища и съкровища, изследващи подземия.",
                    MinPlayers = 2,
                    MaxPlayers = 5,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Sixty,
                    CategoryId = 13,
                },
                "dungeon_board_game.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Among Thieves",
                    Description_EN = "Among Thieves is a strategic heist game where players plan and execute robberies.",
                    Description_BG = "Among Thieves е стратегическа игра за обири, където играчите планират и изпълняват кражби.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.FortyFive,
                    CategoryId = 13,
                },
                "among_thieves.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Here to Slay",
                    Description_EN = "Here to Slay is a strategic card game where players recruit heroes, battle monsters, and slay opponents to win.",
                    Description_BG = "Here to Slay е стратегическа игра с карти, където играчите набират герои, сражават се с чудовища и побеждават противници.",
                    MinPlayers = 2,
                    MaxPlayers = 6,
                    MinAge = 14,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "here_to_slay.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "In Too Deep",
                    Description_EN = "In Too Deep is a cooperative card game about underwater exploration and escaping hazards.",
                    Description_BG = "In Too Deep е кооперативна игра с карти за подводно изследване и избягване на опасности.",
                    MinPlayers = 2,
                    MaxPlayers = 5,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "in_too_deep.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "On a Scale of One to Trex",
                    Description_EN = "On a Scale of One to Trex is a family-friendly party game where players rate outrageous scenarios.",
                    Description_BG = "On a Scale of One to Trex е забавна семейна игра, в която играчите оценяват невероятни сценарии.",
                    MinPlayers = 3,
                    MaxPlayers = 8,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Twenty,
                    CategoryId = 13,
                },
                "on_a_scale_of_one_to_trex.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Discover: Lands Unknown",
                    Description_EN = "Discover: Lands Unknown is an exploration board game where players uncover and colonize new lands.",
                    Description_BG = "Discover: Lands Unknown е изследователска настолна игра, в която играчите откриват и колонизират нови земи.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 12,
                    AveragePlaytime = GameAveragePlaytime.Sixty,
                    CategoryId = 13,
                },
                "discover_lands_unkown.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Comanauts",
                    Description_EN = "Comanauts is a light strategy game where players compete to explore space and gather resources.",
                    Description_BG = "Comanauts е стратегическа игра, в която играчите изследват космоса и събират ресурси.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "comanauts.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Round the World",
                    Description_EN = "Round the World is a travel-themed board game where players race to visit continents and collect points.",
                    Description_BG = "Round the World е настолна игра с тема пътувания, в която играчите се състезават да посетят континенти и да събират точки.",
                    MinPlayers = 2,
                    MaxPlayers = 6,
                    MinAge = 8,
                    AveragePlaytime = GameAveragePlaytime.FortyFive,
                    CategoryId = 13,
                },
                "round_the_word.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "A War of Whispers",
                    Description_EN = "A War of Whispers is a secret influence board game where players manipulate leaders to gain power.",
                    Description_BG = "A War of Whispers е настолна игра със скрита манипулация, където играчите влияят на лидери, за да спечелят власт.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 12,
                    AveragePlaytime = GameAveragePlaytime.FortyFive,
                    CategoryId = 13,
                },
                "a_war_of_whispers.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "The Arrival",
                    Description_EN = "The Arrival is an adventure board game where players navigate new lands and encounter challenges.",
                    Description_BG = "The Arrival е приключенска настолна игра, в която играчите изследват нови земи и срещат предизвикателства.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Sixty,
                    CategoryId = 13,
                },
                "the_arrival.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Master F Orion",
                    Description_EN = "Master F Orion is a space strategy board game with exploration, trade, and combat mechanics.",
                    Description_BG = "Master F Orion е стратегическа настолна игра в космоса с изследване, търговия и бойни механики.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 12,
                    AveragePlaytime = GameAveragePlaytime.Sixty,
                    CategoryId = 13,
                },
                "master_f_orion.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Skyward",
                    Description_EN = "Skyward is a tactical board game where players manage resources and control territories in the skies.",
                    Description_BG = "Skyward е тактическа настолна игра, в която играчите управляват ресурси и контролират територии в небето.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.FortyFive,
                    CategoryId = 13,
                },
                "skyward.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Exploding Kittens",
                    Description_EN = "Exploding Kittens is a light, fast-paced card game full of strategy and explosive fun.",
                    Description_BG = "Exploding Kittens е лека, бърза игра с карти, пълна със стратегия и експлозивно забавление.",
                    MinPlayers = 2,
                    MaxPlayers = 5,
                    MinAge = 7,
                    AveragePlaytime = GameAveragePlaytime.Fifteen,
                    CategoryId = 13,
                },
                "exploding_kittens.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "7 Wonders Duel",
                    Description_EN = "7 Wonders Duel is a two-player card game where players build civilizations and compete for supremacy.",
                    Description_BG = "7 Wonders Duel е двама играчи игра с карти, в която играчите изграждат цивилизации и се състезават за превъзходство.",
                    MinPlayers = 2,
                    MaxPlayers = 2,
                    MinAge = 10,
                    AveragePlaytime = GameAveragePlaytime.Thirty,
                    CategoryId = 13,
                },
                "7_wonders_duel.jpg"
            ),
            new SeedGameData(
                new Game
                {
                    Name = "Battalia: The Creation",
                    Description_EN = "Battalia: The Creation is a strategy board game where players control armies and compete for dominance.",
                    Description_BG = "Battalia: The Creation е стратегическа настолна игра, в която играчите управляват армии и се състезават за превъзходство.",
                    MinPlayers = 2,
                    MaxPlayers = 4,
                    MinAge = 12,
                    AveragePlaytime = GameAveragePlaytime.Sixty,
                    CategoryId = 13,
                },
                "battalia_the_creation.jpg"
            ),
        };

        using (var context = await contextFactory.CreateDbContextAsync(CancellationToken.None))
        {
            foreach (var game in games)
            {
                var exists = await context.Games.AnyAsync(x => x.Name == game.Game.Name);
                if (exists)
                    continue;

                var imageUrl = this.fileManagerClient.GetPublicUrl(FileManagerFolders.Seed.ToString(), game.FileName);
                game.Game.ImageUrl = imageUrl;
                await context.Games.AddAsync(game.Game, CancellationToken.None);

                await context.GameInventories
                    .AddAsync(new GameInventory
                    {
                        Game = game.Game,
                        AvailableCopies = 1,
                        TotalCopies = 1,
                    }, CancellationToken.None);
            }

            await context.SaveChangesAsync(CancellationToken.None);
        }
    }
}

public record SeedGameData(Game Game, string FileName);
