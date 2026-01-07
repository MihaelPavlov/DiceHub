namespace DH.Domain.Adapters.ChatHub;

public interface IChatHubClient
{
    Task SendMessageToGroup(int roomId, string message);
    Task ConnectToGroup(int roomId);
}
