using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.Authentication.Interfaces;
using DH.Domain.Adapters.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Domain;

public static class DomainDIModule
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
            => services;

}