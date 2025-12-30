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
            Language = SupportLanguages.EN.ToString(),
            TemplateName = EmailType.RegistrationEmailConfirmation.ToString(),
            Subject = "Confirm your email address",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Confirm Your Email</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #20232a;
      font-family: Arial, sans-serif;
      color: white;
    ""
  >
    <table
      width=""100%""
      cellpadding=""0""
      cellspacing=""0""
      role=""presentation""
      style=""background-color: #20232a; padding: 20px 0""
    >
      <tr>
        <td align=""center"">
          <table
            width=""500""
            cellpadding=""0""
            cellspacing=""0""
            role=""presentation""
            style=""
              background-color: #20232a;
              padding: 20px;
              border-radius: 8px;
              box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
            ""
          >
            <!-- Icon -->
            <tr>
              <td align=""center"">
                <div
                  style=""
                    width: 100px;
                    height: 120px;
                    display: flex;
                    justify-content: center;
                    align-items: center;
                  ""
                >
                  <img
                    src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png""
                    width=""150""
                    height=""auto""
                    alt=""icon""
                    style=""display: block""
                  />
                </div>
              </td>
            </tr>

            <!-- Header -->
            <tr>
              <td
                align=""center""
                style=""font-size: 22px; font-weight: bold; padding-bottom: 20px""
              >
                {{ClubName}}
              </td>
            </tr>

            <!-- Content -->
            <tr>
              <td style=""font-size: 16px; line-height: 1.6"">
                <p>Hello,</p>
                <p>Thank you for registering at {{ClubName}}!</p>
                <p>Please confirm your email by clicking the button below:</p>

                <div style=""text-align: center; margin: 30px 0"">
                  <a
                    href=""{{CallbackUrl}}""
                    style=""
                      background-color: #75a0ff;
                      color: white;
                      padding: 12px 30px;
                      border-radius: 30px;
                      text-decoration: none;
                      font-weight: bold;
                      display: inline-block;
                    ""
                  >
                    Confirm Email
                  </a>
                </div>

                <p>If you did not sign up, you can ignore this email.</p>
                <p>This link will expire in 24 hours for security reasons.</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>

"
        },
        new ()
        {
            Id = 2,
            Language = SupportLanguages.BG.ToString(),
            TemplateName = EmailType.RegistrationEmailConfirmation.ToString(),
            Subject = "Потвърдете своя имейл адрес",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
<head>
  <meta charset=""UTF-8"" />
  <title>Потвърдете своя имейл</title>
</head>
<body style=""
  margin: 0;
  padding: 0;
  background-color: #20232a;
  font-family: Arial, sans-serif;
  color: white;
"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color: #20232a; padding: 20px 0"">
    <tr>
      <td align=""center"">
        <table width=""500"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""
          background-color: #20232a;
          padding: 20px;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
        "">
          <tr>
            <td align=""center"">
              <img src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png"" width=""150"" style=""display:block;"" alt=""DiceHub"">
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""font-size: 22px; font-weight: bold; padding-bottom: 20px"">
              {{ClubName}}
            </td>
          </tr>

          <tr>
            <td style=""font-size: 16px; line-height: 1.6"">
              <p>Здравейте,</p>
              <p>Благодарим Ви, че се регистрирахте в {{ClubName}}!</p>
              <p>Моля, потвърдете своя имейл като натиснете бутона по-долу:</p>

              <div style=""text-align: center; margin: 30px 0"">
                <a href=""{{CallbackUrl}}"" style=""
                  background-color: #75a0ff;
                  color: white;
                  padding: 12px 30px;
                  border-radius: 30px;
                  text-decoration: none;
                  font-weight: bold;
                  display: inline-block;
                "">Потвърди имейл</a>
              </div>

              <p>Ако не сте създавали регистрация, игнорирайте този имейл.</p>
              <p>Линкът е валиден 24 часа заради мерки за сигурност.</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>

"
        },
        new ()
        {
            Id = 3,
            Language = SupportLanguages.EN.ToString(),
            TemplateName = EmailType.ForgotPasswordReset.ToString(),
            Subject = "Reset Your Password",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
<head>
  <meta charset=""UTF-8"" />
  <title>Reset Your Passwrod</title>
</head>
<body style=""
  margin: 0;
  padding: 0;
  background-color: #20232a;
  font-family: Arial, sans-serif;
  color: white;
"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color: #20232a; padding: 20px 0"">
    <tr>
      <td align=""center"">
        <table width=""500"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""
          background-color: #20232a;
          padding: 20px;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
        "">
          <tr>
            <td align=""center"">
              <img src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png"" width=""150"" style=""display:block;"" alt=""DiceHub"">
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""font-size: 22px; font-weight: bold; padding-bottom: 20px"">
              {{ClubName}}
            </td>
          </tr>

          <tr>
            <td style=""font-size: 16px; line-height: 1.6"">
              <p>Hello,</p>
              <p>We received a request to reset the password for your account.</p>
              <p>If you made this request, please click the button below to set a new password:</p>
              <div style=""text-align: center; margin: 30px 0"">
                <a href=""{{CallbackUrl}}"" style=""
                  background-color: #75a0ff;
                  color: white;
                  padding: 12px 30px;
                  border-radius: 30px;
                  text-decoration: none;
                  font-weight: bold;
                  display: inline-block;
                "">Reset Password</a>
              </div>

               <p>If you did not request a password reset, you can safely ignore this email.</p>
               <p>This link will expire in 24 hours for security reasons.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"
        },
        new ()
        {
            Id = 4,
            Language = SupportLanguages.BG.ToString(),
            TemplateName = EmailType.ForgotPasswordReset.ToString(),
            Subject = "Възстановяване на Парола",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
<head>
  <meta charset=""UTF-8"" />
  <title>Възстановяване на Парола</title>
</head>
<body style=""
  margin: 0;
  padding: 0;
  background-color: #20232a;
  font-family: Arial, sans-serif;
  color: white;
"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color: #20232a; padding: 20px 0"">
    <tr>
      <td align=""center"">
        <table width=""500"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""
          background-color: #20232a;
          padding: 20px;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
        "">
          <tr>
            <td align=""center"">
              <img src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png"" width=""150"" style=""display:block;"" alt=""DiceHub"">
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""font-size: 22px; font-weight: bold; padding-bottom: 20px"">
              {{ClubName}}
            </td>
          </tr>

          <tr>
            <td style=""font-size: 16px; line-height: 1.6"">
              <p>Здравейте,</p>
              <p>Получихме заявка за възтановяване на паролата за вашия акаунт.</p>
              <p>Ако вие сте направили тази заявка, моля натиснете бутона по-долу, за да зададете нова парола:</p>
              <div style=""text-align: center; margin: 30px 0"">
                <a href=""{{CallbackUrl}}"" style=""
                  background-color: #75a0ff;
                  color: white;
                  padding: 12px 30px;
                  border-radius: 30px;
                  text-decoration: none;
                  font-weight: bold;
                  display: inline-block;
                "">Нулирай парола</a>
              </div>

               <p>Ако не сте заявили възтановяване на паролата, може спокойно да игнорирате този имейл.</p>
               <p>Този линк ще изтече след 24 часа поради съображения за сигурност.</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"
        },
        new ()
        {
            Id = 5,
            Language = SupportLanguages.EN.ToString(),
            TemplateName = EmailType.EmployeePasswordCreation.ToString(),
            Subject = "Welcome to {{ClubName}} – Create Your Account Password",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
<head>
  <meta charset=""UTF-8"" />
  <title>Create your account password</title>
</head>
<body style=""
  margin: 0;
  padding: 0;
  background-color: #20232a;
  font-family: Arial, sans-serif;
  color: white;
"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color: #20232a; padding: 20px 0"">
    <tr>
      <td align=""center"">
        <table width=""500"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""
          background-color: #20232a;
          padding: 20px;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
        "">
          <tr>
            <td align=""center"">
              <img src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png"" width=""150"" style=""display:block;"" alt=""DiceHub"">
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""font-size: 22px; font-weight: bold; padding-bottom: 20px"">
              {{ClubName}}
            </td>
          </tr>

          <tr>
            <td style=""font-size: 16px; line-height: 1.6"">
                <p>Hello,</p>
                <p>Your employee account at <strong>{{ClubName}}</strong> has been created.</p>
                <p>To access the system, you need to set up your password.</p>
                <p>Before you can do that, we need to verify your identity.</p>
                <p>Please click the button below to start the setup process. You’ll be asked to confirm your phone number to ensure it matches what your employer provided:</p>
              <div style=""text-align: center; margin: 30px 0"">
                <a href=""{{CallbackUrl}}"" style=""
                  background-color: #75a0ff;
                  color: white;
                  padding: 12px 30px;
                  border-radius: 30px;
                  text-decoration: none;
                  font-weight: bold;
                  display: inline-block;
                "">Set Up Your Password</a>
              </div>

                <p>This link is valid for 24 hours. If it expires, contact your administrator to request a new one.</p>
                <p>Welcome aboard!</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>
"
        },
        new ()
        {
            Id = 6,
            Language = SupportLanguages.BG.ToString(),
            TemplateName = EmailType.EmployeePasswordCreation.ToString(),
            Subject = "Добре Дошъл в екипа  на {{ClubName}} – Създай своята акаунт парола",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
<head>
  <meta charset=""UTF-8"" />
  <title>Създайте парола за своя акаунт</title>
</head>
<body style=""
  margin: 0;
  padding: 0;
  background-color: #20232a;
  font-family: Arial, sans-serif;
  color: white;
"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""background-color: #20232a; padding: 20px 0"">
    <tr>
      <td align=""center"">
        <table width=""500"" cellpadding=""0"" cellspacing=""0"" role=""presentation"" style=""
          background-color: #20232a;
          padding: 20px;
          border-radius: 8px;
          box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
        "">
          <tr>
            <td align=""center"">
              <img src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png"" width=""150"" style=""display:block;"" alt=""DiceHub"">
            </td>
          </tr>

          <tr>
            <td align=""center"" style=""font-size: 22px; font-weight: bold; padding-bottom: 20px"">
              {{ClubName}}
            </td>
          </tr>

          <tr>
            <td style=""font-size: 16px; line-height: 1.6"">
                <p>Здравейте,</p>
                <p>Вашият служебен акаунт в <strong>{{ClubName}}</strong> беше създаден.</p>
                <p>За да получите достъп до системата, трябва да зададете своя парола.</p>
                <p>Преди да направите това, трябва да потвърдим вашата идентичност.</p>
                <p>Моля, натиснете бутона по-долу, за да започнете процеса. Ще бъдете помолени да потвърдите телефонния си номер, за да се уверим, че съвпада с информацията, предоставена от вашия работодател:</p>
              <div style=""text-align: center; margin: 30px 0"">
                <a href=""{{CallbackUrl}}"" style=""
                  background-color: #75a0ff;
                  color: white;
                  padding: 12px 30px;
                  border-radius: 30px;
                  text-decoration: none;
                  font-weight: bold;
                  display: inline-block;
                "">Задайте своята парола</a>
              </div>

                <p>Този линк е валиден 24 часа. Ако изтече, свържете се с вашия администратор, за да заявите нов.</p>
                <p>Добре дошли в екипа!</p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>

"
        },
        new ()
        {
            Id = 7,
            Language =SupportLanguages.EN.ToString(),
            TemplateName = EmailType.PartnerInquiryRequest.ToString(),
            Subject = "Parnet Inquiry Request",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>New Partner Inquiry</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #20232a;
      font-family: Arial, sans-serif;
      color: white;
    ""
  >
    <table
      width=""100%""
      cellpadding=""0""
      cellspacing=""0""
      role=""presentation""
      style=""background-color: #20232a; padding: 20px 0""
    >
      <tr>
        <td align=""center"">
          <table
            width=""500""
            cellpadding=""0""
            cellspacing=""0""
            role=""presentation""
            style=""
              background-color: #20232a;
              padding: 20px;
              border-radius: 8px;
              box-shadow: 0 4px 12px rgba(0, 2, 1, 1);
            ""
          >
            <!-- LOGO -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png""
                  width=""150""
                  style=""display:block;""
                  alt=""DiceHub""
                />
              </td>
            </tr>

            <!-- HEADER -->
            <tr>
              <td
                align=""center""
                style=""font-size: 22px; font-weight: bold; padding-bottom: 20px""
              >
                New Partner Inquiry
              </td>
            </tr>

            <!-- CONTENT -->
            <tr>
              <td style=""font-size: 16px; line-height: 1.6"">
                <p>Hello,</p>
                <p>
                  You have received a new partnership inquiry via the DiceHub
                  platform.
                </p>
                <p><strong>From:</strong> {{Name}}</p>
                <p><strong>Email:</strong> {{Email}}</p>
                <p><strong>Phone Number:</strong> {{PhoneNumber}}</p>
                <p><strong>Message:</strong></p>
                <p>{{Message}}</p>
                <p>
                  Please reach out to the potential partner to continue the
                  conversation.
                </p>
                <p>— The DiceHub System</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>

"
        },
        new ()
        {
            Id = 8,
            Language =SupportLanguages.BG.ToString(),
            TemplateName = EmailType.PartnerInquiryRequest.ToString(),
            Subject = "Ново запитване за партньорство",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Ново запитване за партньорство</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #20232a;
      font-family: Arial, sans-serif;
      color: white;
    ""
  >
    <table
      width=""100%""
      cellpadding=""0""
      cellspacing=""0""
      role=""presentation""
      style=""background-color: #20232a; padding: 20px 0""
    >
      <tr>
        <td align=""center"">
          <table
            width=""500""
            cellpadding=""0""
            cellspacing=""0""
            role=""presentation""
            style=""
              background-color: #20232a;
              padding: 20px;
              border-radius: 8px;
              box-shadow: 0 4px 12px rgba(0, 2, 1, 1);
            ""
          >
            <!-- LOGО -->
            <tr>
              <td align=""center"">
                <img
                  src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png""
                  width=""150""
                  style=""display:block;""
                  alt=""DiceHub""
                />
              </td>
            </tr>

            <!-- HEADER -->
            <tr>
              <td
                align=""center""
                style=""font-size: 22px; font-weight: bold; padding-bottom: 20px""
              >
                Ново запитване за партньорство
              </td>
            </tr>

            <!-- CONTENT -->
            <tr>
              <td style=""font-size: 16px; line-height: 1.6"">
                <p>Здравейте,</p>
                <p>
                  Получихте ново запитване за партньорство през платформата
                  DiceHub.
                </p>
                <p><strong>От:</strong> {{Name}}</p>
                <p><strong>Имейл:</strong> {{Email}}</p>
                <p><strong>Телефон:</strong> {{PhoneNumber}}</p>
                <p><strong>Съобщение:</strong></p>
                <p>{{Message}}</p>
                <p>
                  Моля, свържете се с потенциалния партньор, за да продължите
                  разговора.
                </p>
                <p>— Системата на DiceHub</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
"
        },
        new ()
        {
            Id = 9,
            Language = SupportLanguages.EN.ToString(),
            TemplateName = EmailType.OwnerPasswordCreation.ToString(),
            Subject = "Welcome Owner of {{ClubName}} – Create Your Account Password",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Create Owner Password</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #20232a;
      font-family: Arial, sans-serif;
      color: white;
    ""
  >
    <table
      width=""100%""
      cellpadding=""0""
      cellspacing=""0""
      role=""presentation""
      style=""background-color: #20232a; padding: 20px 0""
    >
      <tr>
        <td align=""center"">
          <table
            width=""500""
            cellpadding=""0""
            cellspacing=""0""
            role=""presentation""
            style=""
              background-color: #20232a;
              padding: 20px;
              border-radius: 8px;
              box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
            ""
          >
            <tr>
              <td align=""center"">
                <img
                  src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png""
                  width=""150""
                  style=""display: block""
                  alt=""DiceHub""
                />
              </td>
            </tr>

            <tr>
              <td
                align=""center""
                style=""font-size: 22px; font-weight: bold; padding-bottom: 20px""
              >
                {{ClubName}}
              </td>
            </tr>

            <tr>
              <td style=""font-size: 16px; line-height: 1.6"">
                <p>Hello and welcome!</p>
                <p>
                  Your owner account for <strong>{{ClubName}}</strong> has
                  been successfully created.
                </p>
                <p>
                  To get started, you'll need to set your password and verify
                  your identity.
                </p>
                <p>
                  Please click the button below to begin the setup process. As
                  part of the setup, we’ll confirm your phone number to ensure
                  it matches your registration details.
                </p>
                <div style=""text-align: center; margin: 30px 0"">
                  <a
                    href=""{{CallbackUrl}}""
                    style=""
                      background-color: #75a0ff;
                      color: white;
                      padding: 12px 30px;
                      border-radius: 30px;
                      text-decoration: none;
                      font-weight: bold;
                      display: inline-block;
                    ""
                    >Set Up Your Password</a
                  >
                </div>
                <p>
                  This link is valid for 24 hours. If it expires, please contact
                  support to request a new one.
                </p>
                <p>
                  We’re excited to have you on board and look forward to
                  supporting your club!
                </p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
"
        },
        new()
        {
            Id = 10,
            Language = SupportLanguages.BG.ToString(),
            TemplateName = EmailType.OwnerPasswordCreation.ToString(),
            Subject = "Добре дошъл, собственик на {{ClubName}} – Създайте своята парола",
            TemplateHtml = @"
<!DOCTYPE html>
<html lang=""bg"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Създаване на парола за собственик</title>
  </head>
  <body
    style=""
      margin: 0;
      padding: 0;
      background-color: #20232a;
      font-family: Arial, sans-serif;
      color: white;
    ""
  >
    <table
      width=""100%""
      cellpadding=""0""
      cellspacing=""0""
      role=""presentation""
      style=""background-color: #20232a; padding: 20px 0""
    >
      <tr>
        <td align=""center"">
          <table
            width=""500""
            cellpadding=""0""
            cellspacing=""0""
            role=""presentation""
            style=""
              background-color: #20232a;
              padding: 20px;
              border-radius: 8px;
              box-shadow: 0 4px 12px rgba(0, 0, 0, 0.5);
            ""
          >
            <tr>
              <td align=""center"">
                <img
                  src=""https://ifjmepvyiksirgheyqyw.supabase.co/storage/v1/object/public/dicehub/Email/dicehub_logo_1.png""
                  width=""150""
                  style=""display: block""
                  alt=""DiceHub""
                />
              </td>
            </tr>

            <tr>
              <td
                align=""center""
                style=""font-size: 22px; font-weight: bold; padding-bottom: 20px""
              >
                {{ClubName}}
              </td>
            </tr>

            <tr>
              <td style=""font-size: 16px; line-height: 1.6"">
                <p>Здравейте и добре дошли!</p>
                <p>
                  Вашият акаунт собственик на <strong>{{ClubName}}</strong>
                  беше успешно създаден.
                </p>
                <p>
                  За да започнете, е необходимо да зададете своята парола и да
                  потвърдите своята идентичност.
                </p>
                <p>
                  Моля, натиснете бутона по-долу, за да започнете процеса на
                  настройка. Като част от него ще бъде извършена проверка на
                  вашия телефонен номер, за да се уверим, че съвпада с данните,
                  предоставени при регистрацията.
                </p>
                <div style=""text-align: center; margin: 30px 0"">
                  <a
                    href=""{{CallbackUrl}}""
                    style=""
                      background-color: #75a0ff;
                      color: white;
                      padding: 12px 30px;
                      border-radius: 30px;
                      text-decoration: none;
                      font-weight: bold;
                      display: inline-block;
                    ""
                    >Задайте своята парола</a
                  >
                </div>
                <p>
                  Този линк е валиден 24 часа. Ако изтече, моля свържете се с
                  поддръжката, за да получите нов.
                </p>
                <p>
                  Радваме се, че сте част от платформата и с нетърпение очакваме
                  да подкрепим вашия клуб!
                </p>
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
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
            Description_EN = "Play your marked favorite game multiple times. Each session with your favorite title counts toward the challenge. If you don't have a liked/favorite games, random game will be added for this challenge",
            Description_BG = "Играй своята любима игра няколко пъти. Всяка сесия с избраната любима игра се брои за напредъка. Ако нямаш харесани/любими игри ще бъде добавена игра на случаен принцип.",
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
            Description_EN = "Maintain a top 3 position on the overall Challenge Leaderboard for consecutive days. Progress is tracked daily, skipping the club's days off. Attempts represent days for this challenge",
            Description_BG = "Запази място в топ 3 на общата класация за предизвикателства няколко дни подред. Напредъкът се следи ежедневно, като се пропускат почивните дни на клуба. Опити е в контекста на дни за това предизвикателство",
            Type = UniversalChallengeType.Top3ChallengeLeaderboard,
            Attempts = 3
        }
    };
}
