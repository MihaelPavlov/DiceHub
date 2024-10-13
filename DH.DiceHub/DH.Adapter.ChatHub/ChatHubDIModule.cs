using DH.Domain.Adapters.ChatHub;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.ChatHub;

public static class ChatHubDIModule
{
    public static void AddChatHubAdapter(
        this IServiceCollection services)
        => services
            .AddScoped<IChatHubClient, ChatHubClient>()
            .AddSignalR();
}
