using DH.Application.Games.Commands.Games;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Application;

public static class ApplicationDIModule
{
    public static void AddApplication(
        this IServiceCollection services)
        => services
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreateGameCommand).Assembly));

}