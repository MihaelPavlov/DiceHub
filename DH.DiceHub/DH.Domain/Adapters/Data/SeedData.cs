using DH.Domain.Adapters.Email;
using DH.Domain.Entities;
using DH.Domain.Enums;
using DH.Domain.Models.ChallengeModels.Commands;
using DH.Domain.Models.GameModels.Commands;
using DH.Domain.Models.RewardModels.Commands;

namespace DH.Domain.Adapters.Data;

public static class SeedData
{
    public static readonly List<GameCategory> GAME_CATEGORIES = new List<GameCategory>
    {
        new() { Id = 1, Name = "Strategy" },
        new() { Id = 2, Name = "Economy" },
        new() { Id = 3, Name = "Card" },
        new() { Id = 4, Name = "Adventure" },
        new() { Id = 5, Name = "Puzzle" },
        new() { Id = 6, Name = "Family" },
        new() { Id = 7, Name = "Party" },
        new() { Id = 8, Name = "Fantasy" },
        new() { Id = 9, Name = "Science Fiction" },
        new() { Id = 10, Name = "Trivia" },
        new() { Id = 11, Name = "Horror" },
        new() { Id = 12, Name = "Abstract" },
        new() { Id = 13, Name = "Warfare" },
        new() { Id = 14, Name = "Dexterity" },
        new() { Id = 15, Name = "Racing" },
        new() { Id = 16, Name = "Historical" },
        new() { Id = 17, Name = "Exploration" },
        new() { Id = 18, Name = "Resource Management" },
        new() { Id = 19, Name = "Cooperative" },
        new() { Id = 20, Name = "Deck Building" },
    };

    public static readonly List<EmailTemplate> EMAIL_TEMPLATES = new List<EmailTemplate>
    {
        new ()
        {
            Id = 1,
            TemplateName = EmailType.RegistrationEmailConfirmation.ToString(),
            Subject = "Confirm your email address",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Confirm Your Email</title>
    <style>
      body {
        background-color: #20232a;
        margin: 0;
        padding: 0;
        color: white;
      }

      .wrapper {
        max-width: 500px;
        max-height: 700px;
        display: flex;
        flex-direction: column;
        justify-content: center;
        margin: 2rem auto;
        padding: 2rem;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 2, 1, 1);
      }

      .interactive-option {
        display: flex;
        justify-content: center;
        margin-bottom: 1rem;
      }

      .icon_wrapper {
        width: 5rem;
        height: 5rem;
        border-radius: 50px;
        background: #75a0ff;
        box-shadow: 0 0 0 12px #20232a;
        display: flex;
        justify-content: center;
        align-items: center;
      }

      .icon {
        width: 3rem;
        height: 3rem;
        fill: white;
      }

      .header {
        text-align: center;
        font-size: 1.5rem;
        margin-bottom: 2rem;
      }

      .content p {
        font-size: 1rem;
        line-height: 1.6;
      }

      .link {
        text-align: center;
        margin: 1rem;
      }

      .link a {
        appearance: none;
        text-decoration: none;
        border: none;
        border-radius: 2rem;
        color: white;
        cursor: pointer;
        font-weight: 600;
        padding: 0.65rem 2rem;
        font-size: 1rem;
        background-color: #75a0ff;
      }
    </style>
  </head>
  <body>
    <div class=""wrapper"">
        <div class=""interactive-option"">
          <div class=""icon_wrapper"">
            <svg class=""icon"" viewBox=""0 -960 960 960"">
              <path
                d=""M750.54-116.15 638.15-229.31l-88 88-11.07-11.07q-26.39-26.39-26.39-63.47 0-37.07 26.39-63.46l145.61-145.61q26.39-26.39 63.46-26.39 37.08 0 63.47 26.39l11.07 11.07-88 88 113.16 113.39q13.69 13.69 13.69 31.61 0 17.93-13.69 31.62l-33.08 33.08q-13.69 13.69-32.12 13.69-18.42 0-32.11-13.69ZM853-735.92 408.31-290.46l24.61 25.38q16.39 26.39 13.39 57.46-3 31.08-26.39 54.47l-11.07 11.07-88-88-113.39 114.16q-13.69 13.69-31.61 13.69-17.93 0-31.62-13.69L110.15-150q-13.69-13.69-13.69-31.62 0-17.92 13.69-31.61l114.16-113.39-88-88 11.07-11.07q23.39-23.39 54.97-26.39 31.57-3 57.96 14.39l26.15 25.38L731.92-857H853v121.08ZM333-601l14.23-15.77 14-14-14 14L333-601Zm-46.31 45.54L107-735.92V-857h121.08L407-677.31l-45.77 46.54L201-791h-28v28l160 162-46.31 45.54ZM363-337l424-426v-28h-28L334-366l29 29Zm0 0-13.23-15.77L334-366l15.77 13.23L363-337Z""
              />
            </svg>
          </div>
        </div>
        <div class=""header"">{{ClubName}}</div>
      <div class=""content"">
        <p>Hello,</p>
        <p>Thank you for registering at {{ClubName}}!</p>
        <p>Please confirm your email by clicking the button below:</p>
        <p class=""link""><a href=""{{CallbackUrl}}"">Confirm Email</a></p>
        <p>If you did not sign up, you can ignore this email.</p>
      </div>
    </div>
  </body>
</html>
"
        }
    };

    public static readonly List<CreateGameDto> GAME_LIST_DTOS = new List<CreateGameDto>
    {
        new CreateGameDto { CategoryId = 1, Name = "Catan", Description = "A strategy game about resource trading and expansion.", MinAge = 10, MinPlayers = 3, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Fifty },
        new CreateGameDto { CategoryId = 1, Name = "Solitaire", Description = "A classic card game where the goal is to arrange all cards in order.", MinAge = 8, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 2, Name = "Monopoly", Description = "The classic property trading game.", MinAge = 8, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 2, Name = "Stardew Valley", Description = "A farming simulation game where you can grow crops, raise animals, and explore.", MinAge = 10, MinPlayers = 1, MaxPlayers = 3, AveragePlaytime = GameAveragePlaytime.Seventy },
        new CreateGameDto { CategoryId = 3, Name = "Uno", Description = "A classic card game where players try to match colors and numbers.", MinAge = 7, MinPlayers = 2, MaxPlayers = 10, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 3, Name = "The Witcher 3: Wild Hunt", Description = "An open-world RPG where you play as Geralt of Rivia on a quest to find his daughter.", MinAge = 18, MinPlayers = 1, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.NinetyFive },
        new CreateGameDto { CategoryId = 4, Name = "Gloomhaven", Description = "A cooperative game where players explore a dark fantasy world.", MinAge = 14, MinPlayers = 1, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.TwentyFive },
        new CreateGameDto { CategoryId = 4, Name = "Celeste", Description = "A platforming game about climbing a mountain and overcoming personal challenges.", MinAge = 10, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 5, Name = "Pandemic", Description = "A cooperative game where players work together to stop global outbreaks.", MinAge = 13, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 5, Name = "Portal 2", Description = "A puzzle-platform game that challenges players to use a portal gun to solve puzzles.", MinAge = 12, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Ninety },
        new CreateGameDto { CategoryId = 6, Name = "Ticket to Ride", Description = "A game about building train routes across the country.", MinAge = 8, MinPlayers = 2, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.Ten },
        new CreateGameDto { CategoryId = 7, Name = "Codenames", Description = "A word-based party game where players guess words based on clues.", MinAge = 14, MinPlayers = 4, MaxPlayers = 8, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 8, Name = "7 Wonders", Description = "A card drafting game that spans three ages of civilization.", MinAge = 10, MinPlayers = 3, MaxPlayers = 7, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 9, Name = "Terraforming Mars", Description = "Players work as corporations to terraform the planet Mars.", MinAge = 12, MinPlayers = 1, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 10, Name = "Splendor", Description = "A game about collecting gems and building a trading empire.", MinAge = 10, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 11, Name = "Risk", Description = "A game of global domination through strategic troop movements.", MinAge = 10, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 12, Name = "Azul", Description = "A tile-laying game where players create beautiful patterns.", MinAge = 8, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 13, Name = "Battleship", Description = "A strategic guessing game where players sink each other's ships.", MinAge = 7, MinPlayers = 2, MaxPlayers = 2, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 14, Name = "The Resistance", Description = "A party game of social deduction and deception.", MinAge = 14, MinPlayers = 5, MaxPlayers = 10, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 15, Name = "Dixit", Description = "A storytelling card game where players use imagination to guess the story.", MinAge = 8, MinPlayers = 3, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 16, Name = "Kingdomino", Description = "A domino-style game where players build their kingdoms.", MinAge = 8, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 17, Name = "Ghost Stories", Description = "A cooperative game where players defend a village from ghosts.", MinAge = 14, MinPlayers = 1, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.FiftyFive },
        new CreateGameDto { CategoryId = 18, Name = "Scythe", Description = "A game set in an alternate-history 1920s Europe.", MinAge = 14, MinPlayers = 1, MaxPlayers = 7, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 19, Name = "Root", Description = "A game of woodland might and right.", MinAge = 10, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Seventy },
        new CreateGameDto { CategoryId = 20, Name = "Coup", Description = "A game of bluffing and deduction.", MinAge = 13, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Twenty, },
    };

    public static readonly List<CreateRewardDto> REWARD_LIST_DTOS = new List<CreateRewardDto>
    {
        new CreateRewardDto { Name = "Free Pass", Description = "A free pass for your next game night.", RequiredPoints = RewardRequiredPoint.Five, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Coca-Cola", Description = "A refreshing Coca-Cola drink.", RequiredPoints = RewardRequiredPoint.Ten, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Sandwich", Description = "A delicious sandwich of your choice.", RequiredPoints = RewardRequiredPoint.Fifteen, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Cake Slice", Description = "A slice of our special homemade cake.", RequiredPoints = RewardRequiredPoint.Twenty, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Chips", Description = "A bag of chips to munch on while you play.", RequiredPoints = RewardRequiredPoint.TwentyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Draft Beer", Description = "A refreshing draft beer to enjoy.", RequiredPoints = RewardRequiredPoint.Thirty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Ice Cream", Description = "A scoop of ice cream in your favorite flavor.", RequiredPoints = RewardRequiredPoint.ThirtyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Soft Drink", Description = "Your choice of a soft drink.", RequiredPoints = RewardRequiredPoint.Forty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Pizza Slice", Description = "A delicious slice of pizza.", RequiredPoints = RewardRequiredPoint.FortyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Free Game Rental", Description = "Rent a game for free for your next visit.", RequiredPoints = RewardRequiredPoint.Fifty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Homemade Cookies", Description = "A pack of freshly baked cookies.", RequiredPoints = RewardRequiredPoint.FiftyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Gourmet Popcorn", Description = "A serving of gourmet popcorn.", RequiredPoints = RewardRequiredPoint.Sixty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Board Game Discount", Description = "A discount on your next board game purchase.", RequiredPoints = RewardRequiredPoint.SixtyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Game Night Special", Description = "A special dish for game night.", RequiredPoints = RewardRequiredPoint.Seventy, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "T-shirt", Description = "A cool board game club T-shirt.", RequiredPoints = RewardRequiredPoint.SeventyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Ultimate Snack Pack", Description = "A variety of snacks to enjoy.", RequiredPoints = RewardRequiredPoint.Eighty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Coffee or Tea", Description = "A warm cup of coffee or tea.", RequiredPoints = RewardRequiredPoint.EightyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Board Game Night Voucher", Description = "A voucher for a board game night with friends.", RequiredPoints = RewardRequiredPoint.Ninety, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Beverage of Choice", Description = "Your choice of any beverage.", RequiredPoints = RewardRequiredPoint.NinetyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Party Platter", Description = "A platter of snacks for your game night.", RequiredPoints = RewardRequiredPoint.OneHundred, Level = RewardLevel.Platinum },
        new CreateRewardDto { Name = "Hot Dog", Description = "Enjoy a classic hot dog.", RequiredPoints = RewardRequiredPoint.Five, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Fruit Juice", Description = "A refreshing fruit juice of your choice.", RequiredPoints = RewardRequiredPoint.Ten, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Pretzel", Description = "A warm, soft pretzel.", RequiredPoints = RewardRequiredPoint.Fifteen, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Cheeseburger", Description = "A juicy cheeseburger to satisfy your hunger.", RequiredPoints = RewardRequiredPoint.Twenty, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name = "Vegetarian Wrap", Description = "A delicious wrap filled with fresh veggies.", RequiredPoints = RewardRequiredPoint.TwentyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Energy Drink", Description = "A boost of energy with an energy drink.", RequiredPoints = RewardRequiredPoint.Thirty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Brownie", Description = "A rich and fudgy brownie.", RequiredPoints = RewardRequiredPoint.ThirtyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Nachos", Description = "A plate of cheesy nachos with toppings.", RequiredPoints = RewardRequiredPoint.Forty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name = "Smoothie", Description = "A healthy fruit smoothie.", RequiredPoints = RewardRequiredPoint.FortyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Chocolate Bar", Description = "A sweet chocolate bar.", RequiredPoints = RewardRequiredPoint.Fifty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Fried Chicken", Description = "Crispy fried chicken pieces.", RequiredPoints = RewardRequiredPoint.FiftyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Mini Tacos", Description = "A serving of mini tacos with salsa.", RequiredPoints = RewardRequiredPoint.Sixty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Gourmet Sandwich", Description = "A gourmet sandwich with premium ingredients.", RequiredPoints = RewardRequiredPoint.SixtyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Chili Cheese Fries", Description = "Fries topped with chili and cheese.", RequiredPoints = RewardRequiredPoint.Seventy, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Sushi Roll", Description = "A fresh sushi roll of your choice.", RequiredPoints = RewardRequiredPoint.SeventyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Cheese Platter", Description = "An assorted cheese platter.", RequiredPoints = RewardRequiredPoint.Eighty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Fruit Salad", Description = "A healthy fruit salad mix.", RequiredPoints = RewardRequiredPoint.EightyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Loyalty Card", Description = "Get points towards free games.", RequiredPoints = RewardRequiredPoint.Ninety, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Game Night Host", Description = "Host a game night with friends for free.", RequiredPoints = RewardRequiredPoint.NinetyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name = "Party Package", Description = "A party package including games and snacks.", RequiredPoints = RewardRequiredPoint.OneHundred, Level = RewardLevel.Platinum }
    };


    public static readonly List<CreateChallengeDto> CHALLENGE_LIST_DTOS = new List<CreateChallengeDto>
    {
        new CreateChallengeDto { Description = "Win 3 games in a row without losing your mind!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Try a new board game and pretend you're an expert!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Play with at least 4 different players—make new friends or annoy old ones!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Win the game night tournament and brag about it for a month!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Complete 5 challenges this month—sounds easy, right?", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 5 },
        new CreateChallengeDto { Description = "Share your favorite game strategy and make everyone roll their eyes!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Organize a game night and make sure snacks are the real MVP!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Attend a board game club event this week—free snacks included!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Win a game with a score difference of 20 points or more—easy peasy!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Teach someone how to play, and act like a game guru!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 5 },
        new CreateChallengeDto { Description = "Play 3 different games this week—variety is the spice of life!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Create a unique game night theme—mandatory costumes encouraged!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Record a video playing your favorite game—go viral or go home!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Participate in a game night charity event—play for a cause!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Host a game night for friends and family—may the odds be ever in your favor!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 5 },
        new CreateChallengeDto { Description = "Play a game without using any strategy guide—risk it for the biscuit!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Bring a new player to the club—bonus points if they're confused!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Complete a game within a set time limit—time's ticking!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Win a game with a special condition—play it like a pro!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Attend a game review session and pretend you know everything!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 5 },
        new CreateChallengeDto { Description = "Score the most points in a single game this week—no pressure!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Beat your own high score while wearing a silly hat!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Play a game while using only one hand—extra challenge mode!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Teach someone a new game and pretend you're a game master!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Create your own board game rule—let the chaos begin!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Lose a game on purpose and still act like you won!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 5 },
        new CreateChallengeDto { Description = "Dress up as your favorite game character for game night!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 6 },
        new CreateChallengeDto { Description = "Play a game in a different language—confusion guaranteed!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 7 },
        new CreateChallengeDto { Description = "Finish a game without laughing—even if it's hilarious!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 8 },
        new CreateChallengeDto { Description = "Create a game-themed snack for your next game night!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 9 },
        new CreateChallengeDto { Description = "Play a game blindfolded—trust your instincts!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 10 },
        new CreateChallengeDto { Description = "Use a random rule generator for your next game night!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 11 },
        new CreateChallengeDto { Description = "Bring a mystery item to the game and make it part of the game!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 12 },
        new CreateChallengeDto { Description = "Host a game night with a twist—every player must speak in accents!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 13 },
        new CreateChallengeDto { Description = "Win a game while having a ridiculous debate with another player!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 14 },
        new CreateChallengeDto { Description = "Complete a game with someone who has never played it before!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 15 },
        new CreateChallengeDto { Description = "Play the game while standing on one leg—balance is key!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 16 },
        new CreateChallengeDto { Description = "Make a trophy for the winner of your next game night!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 17 },
        new CreateChallengeDto { Description = "Create a rap about your favorite game—bonus points for style!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 18 },
        new CreateChallengeDto { Description = "Play a game using only emojis to describe your moves!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 19 },
        new CreateChallengeDto { Description = "Film yourself playing and narrating like a sports commentator!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 20 },
        new CreateChallengeDto { Description = "Try to win while enforcing a silly rule—like no smiling!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 21 },
        new CreateChallengeDto { Description = "Create a game night playlist to set the mood!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 22 },
        new CreateChallengeDto { Description = "Participate in a game without using any strategy—just go with the flow!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 23 },
        new CreateChallengeDto { Description = "Win a game while explaining the rules as if you’re on a cooking show!", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 24 },
        new CreateChallengeDto { Description = "Bring a friend who doesn’t like board games—can you convert them?", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 25 },
        new CreateChallengeDto { Description = "Play a game using only your non-dominant hand—good luck!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 1 },
        new CreateChallengeDto { Description = "Make a game night rule that everyone must follow—no exceptions!", RewardPoints = ChallengeRewardPoint.Fifteen, Attempts = 3, Type = ChallengeType.Weekly, GameId = 2 },
        new CreateChallengeDto { Description = "Finish a game without any snacks—can you survive?", RewardPoints = ChallengeRewardPoint.Twenty, Attempts = 4, Type = ChallengeType.Weekly, GameId = 3 },
        new CreateChallengeDto { Description = "Create a fun challenge for the next game night—get creative!", RewardPoints = ChallengeRewardPoint.Five, Attempts = 1, Type = ChallengeType.Weekly, GameId = 4 },
        new CreateChallengeDto { Description = "Challenge another player to a best-of-three duel in your favorite game!", RewardPoints = ChallengeRewardPoint.Ten, Attempts = 2, Type = ChallengeType.Weekly, GameId = 5 },
    };
}
