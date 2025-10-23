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
        <p>This link will expire in 24 hours for security reasons.</p>
      </div>
    </div>
  </body>
</html>
"
        },
        new ()
        {
            Id = 2,
            TemplateName = EmailType.ForgotPasswordReset.ToString(),
            Subject = "Reset Your Password",
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
        <p>We received a request to reset the password for your account.</p>
        <p>If you made this request, please click the button below to set a new password:</p>
        <p class=""link""><a href=""{{CallbackUrl}}"">Reset Password</a></p>
        <p>If you did not request a password reset, you can safely ignore this email.</p>
        <p>This link will expire in 24 hours for security reasons.</p>
      </div>
    </div>
  </body>
</html>
"
        },
        new ()
        {
            Id = 3,
            TemplateName = EmailType.EmployeePasswordCreation.ToString(),
            Subject = "Welcome to {{ClubName}} – Create Your Account Password",
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
        padding: 0.65rem 1rem;
        font-size: 1rem;
        background-color: #75a0ff;
        display: flex;
        justify-content: center;
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
        <p>Your employee account at <strong>{{ClubName}}</strong> has been created.</p>
        <p>To access the system, you need to set up your password.</p>
        <p>Before you can do that, we need to verify your identity.</p>
        <p>Please click the button below to start the setup process. You’ll be asked to confirm your phone number to ensure it matches what your employer provided:</p>
        <p class=""link""><a href=""{{CreatePasswordUrl}}"">Set Up Your Password</a></p>
        <p>This link is valid for 24 hours. If it expires, contact your administrator to request a new one.</p>
        <p>Welcome aboard!</p>
      </div>
    </div>
  </body>
</html>
"
        },
        new ()
        {
            Id = 4,
            TemplateName = EmailType.PartnerInquiryRequest.ToString(),
            Subject = "Parnet Inquiry Request",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>New Partner Inquiry</title>
    <style>
      body {
        background-color: #20232a;
        margin: 0;
        padding: 0;
        color: white;
      }

      .wrapper {
        max-width: 500px;
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
    </style>
  </head>
  <body>
     <div class=""wrapper"">
      <div class=""interactive-option"">
        <div class=""icon_wrapper"">
          <svg class=""icon"" viewBox=""0 -960 960 960"">
            <path d=""..."" /> <!-- Keep the icon path as-is -->
          </svg>
        </div>
      </div>
      <div class=""header"">New Partner Inquiry</div>
      <div class=""content"">
        <p>Hello,</p>
        <p>You have received a new partnership inquiry via the DiceHub platform.</p>
        <p><strong>From:</strong> {{Name}}</p>
        <p><strong>Email:</strong> {{Email}}</p>
        <p><strong>Phone Number:</strong> {{PhoneNumber}}</p>
        <p><strong>Message:</strong></p>
        <p>{{Message}}</p>
        <p>Please reach out to the potential partner to continue the conversation.</p>
        <p>— The DiceHub System</p>
      </div>
    </div>
  </body>
</html>
"
        },
        new ()
        {
            Id = 5,
            TemplateName = EmailType.OwnerPasswordCreation.ToString(),
            Subject = "Welcome Owner of {{ClubName}} – Create Your Account Password",
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
        padding: 0.65rem 1rem;
        font-size: 1rem;
        background-color: #75a0ff;
        display: flex;
        justify-content: center;
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
         <p>Hello and welcome!</p>
            <p>Your owner account for <strong>{{ClubName}}</strong> has been successfully created.</p>
            <p>To get started, you'll need to set your password and verify your identity.</p>
            <p>Please click the button below to begin the setup process. As part of the setup, we’ll confirm your phone number to ensure it matches your registration details.</p>
            <p class=""link""><a href=""{{CreatePasswordUrl}}"">Set Up Your Password</a></p>
            <p>This link is valid for 24 hours. If it expires, please contact support to request a new one.</p>
            <p>We’re excited to have you on board and look forward to supporting your club!</p>
      </div>
    </div>
  </body>
</html>
"
        },
    };

    public static readonly List<CreateGameDto> GAME_LIST_DTOS = new List<CreateGameDto>
    {
        new CreateGameDto { CategoryId = 1, Name = "Catan", Description_EN = "A strategy game about resource trading and expansion.", MinAge = 10, MinPlayers = 3, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Fifty },
        new CreateGameDto { CategoryId = 1, Name = "Solitaire", Description_EN = "A classic card game where the goal is to arrange all cards in order.", MinAge = 8, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 2, Name = "Monopoly", Description_EN = "The classic property trading game.", MinAge = 8, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 2, Name = "Stardew Valley", Description_EN = "A farming simulation game where you can grow crops, raise animals, and explore.", MinAge = 10, MinPlayers = 1, MaxPlayers = 3, AveragePlaytime = GameAveragePlaytime.Seventy },
        new CreateGameDto { CategoryId = 3, Name = "Uno", Description_EN = "A classic card game where players try to match colors and numbers.", MinAge = 7, MinPlayers = 2, MaxPlayers = 10, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 3, Name = "The Witcher 3: Wild Hunt", Description_EN = "An open-world RPG where you play as Geralt of Rivia on a quest to find his daughter.", MinAge = 18, MinPlayers = 1, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.NinetyFive },
        new CreateGameDto { CategoryId = 4, Name = "Gloomhaven", Description_EN = "A cooperative game where players explore a dark fantasy world.", MinAge = 14, MinPlayers = 1, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.TwentyFive },
        new CreateGameDto { CategoryId = 4, Name = "Celeste", Description_EN = "A platforming game about climbing a mountain and overcoming personal challenges.", MinAge = 10, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 5, Name = "Pandemic", Description_EN = "A cooperative game where players work together to stop global outbreaks.", MinAge = 13, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 5, Name = "Portal 2", Description_EN = "A puzzle-platform game that challenges players to use a portal gun to solve puzzles.", MinAge = 12, MinPlayers = 1, MaxPlayers = 1, AveragePlaytime = GameAveragePlaytime.Ninety },
        new CreateGameDto { CategoryId = 6, Name = "Ticket to Ride", Description_EN = "A game about building train routes across the country.", MinAge = 8, MinPlayers = 2, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.Ten },
        new CreateGameDto { CategoryId = 7, Name = "Codenames", Description_EN = "A word-based party game where players guess words based on clues.", MinAge = 14, MinPlayers = 4, MaxPlayers = 8, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 8, Name = "7 Wonders", Description_EN = "A card drafting game that spans three ages of civilization.", MinAge = 10, MinPlayers = 3, MaxPlayers = 7, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 9, Name = "Terraforming Mars", Description_EN = "Players work as corporations to terraform the planet Mars.", MinAge = 12, MinPlayers = 1, MaxPlayers = 5, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 10, Name = "Splendor", Description_EN = "A game about collecting gems and building a trading empire.", MinAge = 10, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 11, Name = "Risk", Description_EN = "A game of global domination through strategic troop movements.", MinAge = 10, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 12, Name = "Azul", Description_EN = "A tile-laying game where players create beautiful patterns.", MinAge = 8, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 13, Name = "Battleship", Description_EN = "A strategic guessing game where players sink each other's ships.", MinAge = 7, MinPlayers = 2, MaxPlayers = 2, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 14, Name = "The Resistance", Description_EN = "A party game of social deduction and deception.", MinAge = 14, MinPlayers = 5, MaxPlayers = 10, AveragePlaytime = GameAveragePlaytime.Twenty },
        new CreateGameDto { CategoryId = 15, Name = "Dixit", Description_EN = "A storytelling card game where players use imagination to guess the story.", MinAge = 8, MinPlayers = 3, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Fifteen },
        new CreateGameDto { CategoryId = 16, Name = "Kingdomino", Description_EN = "A domino-style game where players build their kingdoms.", MinAge = 8, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 17, Name = "Ghost Stories", Description_EN = "A cooperative game where players defend a village from ghosts.", MinAge = 14, MinPlayers = 1, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.FiftyFive },
        new CreateGameDto { CategoryId = 18, Name = "Scythe", Description_EN = "A game set in an alternate-history 1920s Europe.", MinAge = 14, MinPlayers = 1, MaxPlayers = 7, AveragePlaytime = GameAveragePlaytime.Thirty },
        new CreateGameDto { CategoryId = 19, Name = "Root", Description_EN = "A game of woodland might and right.", MinAge = 10, MinPlayers = 2, MaxPlayers = 4, AveragePlaytime = GameAveragePlaytime.Seventy },
        new CreateGameDto { CategoryId = 20, Name = "Coup", Description_EN = "A game of bluffing and deduction.", MinAge = 13, MinPlayers = 2, MaxPlayers = 6, AveragePlaytime = GameAveragePlaytime.Twenty, },
    };

    public static readonly List<CreateRewardDto> REWARD_LIST_DTOS = new List<CreateRewardDto>
    {
        new CreateRewardDto { Name_EN = "Free Pass", Description_EN = "A free pass for your next game night.", RequiredPoints = RewardRequiredPoint.Five, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Coca-Cola", Description_EN = "A refreshing Coca-Cola drink.", RequiredPoints = RewardRequiredPoint.Ten, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Sandwich", Description_EN = "A delicious sandwich of your choice.", RequiredPoints = RewardRequiredPoint.Fifteen, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Cake Slice", Description_EN = "A slice of our special homemade cake.", RequiredPoints = RewardRequiredPoint.Twenty, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Chips", Description_EN = "A bag of chips to munch on while you play.", RequiredPoints = RewardRequiredPoint.TwentyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Draft Beer", Description_EN = "A refreshing draft beer to enjoy.", RequiredPoints = RewardRequiredPoint.Thirty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Ice Cream", Description_EN = "A scoop of ice cream in your favorite flavor.", RequiredPoints = RewardRequiredPoint.ThirtyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Soft Drink", Description_EN = "Your choice of a soft drink.", RequiredPoints = RewardRequiredPoint.Forty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Pizza Slice", Description_EN = "A delicious slice of pizza.", RequiredPoints = RewardRequiredPoint.FortyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Free Game Rental", Description_EN = "Rent a game for free for your next visit.", RequiredPoints = RewardRequiredPoint.Fifty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Homemade Cookies", Description_EN = "A pack of freshly baked cookies.", RequiredPoints = RewardRequiredPoint.FiftyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Gourmet Popcorn", Description_EN = "A serving of gourmet popcorn.", RequiredPoints = RewardRequiredPoint.Sixty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Board Game Discount", Description_EN = "A discount on your next board game purchase.", RequiredPoints = RewardRequiredPoint.SixtyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Game Night Special", Description_EN = "A special dish for game night.", RequiredPoints = RewardRequiredPoint.Seventy, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "T-shirt", Description_EN = "A cool board game club T-shirt.", RequiredPoints = RewardRequiredPoint.SeventyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Ultimate Snack Pack", Description_EN = "A variety of snacks to enjoy.", RequiredPoints = RewardRequiredPoint.Eighty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Coffee or Tea", Description_EN = "A warm cup of coffee or tea.", RequiredPoints = RewardRequiredPoint.EightyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Board Game Night Voucher", Description_EN = "A voucher for a board game night with friends.", RequiredPoints = RewardRequiredPoint.Ninety, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Beverage of Choice", Description_EN = "Your choice of any beverage.", RequiredPoints = RewardRequiredPoint.NinetyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Party Platter", Description_EN = "A platter of snacks for your game night.", RequiredPoints = RewardRequiredPoint.OneHundred, Level = RewardLevel.Platinum },
        new CreateRewardDto { Name_EN = "Hot Dog", Description_EN = "Enjoy a classic hot dog.", RequiredPoints = RewardRequiredPoint.Five, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Fruit Juice", Description_EN = "A refreshing fruit juice of your choice.", RequiredPoints = RewardRequiredPoint.Ten, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Pretzel", Description_EN = "A warm, soft pretzel.", RequiredPoints = RewardRequiredPoint.Fifteen, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Cheeseburger", Description_EN = "A juicy cheeseburger to satisfy your hunger.", RequiredPoints = RewardRequiredPoint.Twenty, Level = RewardLevel.Bronze },
        new CreateRewardDto { Name_EN = "Vegetarian Wrap", Description_EN = "A delicious wrap filled with fresh veggies.", RequiredPoints = RewardRequiredPoint.TwentyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Energy Drink", Description_EN = "A boost of energy with an energy drink.", RequiredPoints = RewardRequiredPoint.Thirty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Brownie", Description_EN = "A rich and fudgy brownie.", RequiredPoints = RewardRequiredPoint.ThirtyFive, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Nachos", Description_EN = "A plate of cheesy nachos with toppings.", RequiredPoints = RewardRequiredPoint.Forty, Level = RewardLevel.Silver },
        new CreateRewardDto { Name_EN = "Smoothie", Description_EN = "A healthy fruit smoothie.", RequiredPoints = RewardRequiredPoint.FortyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Chocolate Bar", Description_EN = "A sweet chocolate bar.", RequiredPoints = RewardRequiredPoint.Fifty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Fried Chicken", Description_EN = "Crispy fried chicken pieces.", RequiredPoints = RewardRequiredPoint.FiftyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Mini Tacos", Description_EN = "A serving of mini tacos with salsa.", RequiredPoints = RewardRequiredPoint.Sixty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Gourmet Sandwich", Description_EN = "A gourmet sandwich with premium ingredients.", RequiredPoints = RewardRequiredPoint.SixtyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Chili Cheese Fries", Description_EN = "Fries topped with chili and cheese.", RequiredPoints = RewardRequiredPoint.Seventy, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Sushi Roll", Description_EN = "A fresh sushi roll of your choice.", RequiredPoints = RewardRequiredPoint.SeventyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Cheese Platter", Description_EN = "An assorted cheese platter.", RequiredPoints = RewardRequiredPoint.Eighty, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Fruit Salad", Description_EN = "A healthy fruit salad mix.", RequiredPoints = RewardRequiredPoint.EightyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Loyalty Card", Description_EN = "Get points towards free games.", RequiredPoints = RewardRequiredPoint.Ninety, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Game Night Host", Description_EN = "Host a game night with friends for free.", RequiredPoints = RewardRequiredPoint.NinetyFive, Level = RewardLevel.Gold },
        new CreateRewardDto { Name_EN = "Party Package", Description_EN = "A party package including games and snacks.", RequiredPoints = RewardRequiredPoint.OneHundred, Level = RewardLevel.Platinum }
    };


    public static readonly List<CreateChallengeDto> CHALLENGE_LIST_DTOS = new();

    public static readonly List<UniversalChallenge> UNIVERSAL_CHALLENGES = new()
    {
        new()
        {
            Id = 1,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Play X Games",
            Name_BG = "Играй X игри",
            Description_EN = "Play a set number of games during the challenge period. Each completed game session will count towards your progress.",
            Description_BG = "Играй определен брой игри в рамките на периода на предизвикателството. Всяка завършена игрова сесия се брои към напредъка ти.",
            Type = UniversalChallengeType.PlayGames,
            Attempts = 5
        },
        new()
        {
            Id = 2,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Play Game From Meeple Room",
            Name_BG = "Играй игра от Meeple Room",
            Description_EN = "Start and finish a game that was scheduled via a Meeple Room. The game must match the event’s game selection for it to count.",
            Description_BG = "Започни и завърши игра, която е била планирана чрез Meeple Room. Играта трябва да съвпада с избора на събитието, за да се зачете.",
            Type = UniversalChallengeType.JoinMeepleRooms,
            Attempts = 3
        },
        new()
        {
            Id = 3,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Join X Events",
            Name_BG = "Участвай в X събития",
            Description_EN = "Participate in community events hosted at the club. Each event joined adds to your challenge progress.",
            Description_BG = "Участвай в клубни събития. Всяко събитие, в което участваш, добавя точки към напредъка ти.",
            Type = UniversalChallengeType.JoinEvents,
            Attempts = 2
        },
        new()
        {
            Id = 4,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Use X Rewards",
            Name_BG = "Използвай X награди",
            Description_EN = "Redeem and use rewards during your play sessions. Every time you activate a reward QR code, progress is tracked.",
            Description_BG = "Използвай награди по време на игралните сесии. Всеки път когато активираш QR код на награда, напредъкът се отчита.",
            Type = UniversalChallengeType.UseRewards,
            Attempts = 5
        },
        new()
        {
            Id = 5,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Receive X Rewards",
            Name_BG = "Получи X награди",
            Description_EN = "Collect rewards granted by completing other challenges or special tasks. Each reward received increases your progress.",
            Description_BG = "Получавай награди от завършени предизвикателства или специални задачи. Всяка получена награда увеличава напредъка ти.",
            Type = UniversalChallengeType.RewardsGranted,
            Attempts = 3
        },
        new()
        {
            Id = 6,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Buy Items Above X Value",
            Name_BG = "Купи предмети над X стойност",
            Description_EN = "Make purchases from the club shop with a total value above the set minimum. Staff will validate this with a QR code scan.",
            Description_BG = "Направи покупки от магазина на клуба на стойност над минималната. Персоналът ще потвърди със сканиране на QR код.",
            Type = UniversalChallengeType.BuyItems,
            Attempts = 2,
            MinValue = 50m
        },
        new()
        {
            Id = 7,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Play Favorite Game",
            Name_BG = "Играй любима игра",
            Description_EN = "Play your marked favorite game multiple times. Each session with your favorite title counts toward the challenge.",
            Description_BG = "Играй своята любима игра няколко пъти. Всяка сесия с избраната любима игра се брои за напредъка.",
            Type = UniversalChallengeType.PlayFavoriteGame,
            Attempts = 3
        },
        new()
        {
            Id = 8,
            RewardPoints = ChallengeRewardPoint.Ten,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = "Seeder",
            UpdatedBy = "Seeder",
            Name_EN = "Stay in Top 3 Challenge Leaderboard",
            Name_BG = "Остани в топ 3 на класацията",
            Description_EN = "Maintain a top 3 position on the overall Challenge Leaderboard for consecutive days. Progress is tracked daily. Attempts represent days for this challenge",
            Description_BG = "Запази място в топ 3 на общата класация за предизвикателства няколко дни подред. Напредъкът се следи ежедневно. Опити е в контекста на дни за това предизвикателство",
            Type = UniversalChallengeType.Top3ChallengeLeaderboard,
            Attempts = 3
        }
    };
}
