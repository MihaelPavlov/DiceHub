using DH.Domain.Adapters.FileManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;

namespace DH.Adapter.FileManager;

public static class DI
{
    public static IServiceCollection AddFileManager(
    this IServiceCollection services, IConfiguration configuration)
    => services
        .AddScoped<Supabase.Client>(_ =>
        {
            return new Supabase.Client(
                 configuration["SupabaseUrl"],
                 configuration["SupabaseKey"]);
        }).AddScoped<IFileManagerClient, SupabaseStorageClient>();
}

