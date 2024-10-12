using Microsoft.Extensions.DependencyInjection;

namespace DH.Domain;

public static class DomainDIModule
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
            => services;
}