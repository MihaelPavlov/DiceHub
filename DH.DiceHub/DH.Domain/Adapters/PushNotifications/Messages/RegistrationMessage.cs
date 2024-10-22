using DH.Domain.Adapters.PushNotifications.Messages.Common;

namespace DH.Domain.Adapters.PushNotifications.Messages;

public class RegistrationMessage : MessageRequest
{
    public string Username { get; set; }
    public string? WelcomeMessage { get; set; }

    public RegistrationMessage(string username, string? welcomeMessage = null)
    {
        this.Username = username;
        this.WelcomeMessage = welcomeMessage;

        Title = "Welcome to DiceHub!";
        Body = string.IsNullOrEmpty(this.WelcomeMessage) ? $"Hello {this.Username}, thank you for registering!" : this.WelcomeMessage;
    }
}
