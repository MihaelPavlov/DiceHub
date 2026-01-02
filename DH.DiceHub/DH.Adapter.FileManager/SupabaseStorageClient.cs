using DH.Domain.Adapters.FileManager;
using Microsoft.Extensions.Configuration;

namespace DH.Adapter.FileManager;

public class SupabaseStorageClient : IFileManagerClient
{
    readonly Supabase.Client client;
    readonly IConfiguration configuration;

    public SupabaseStorageClient(Supabase.Client client, IConfiguration configuration)
    {
        this.client = client;
        this.configuration = configuration;
    }

    public string GetPublicUrl(string folderName, string fileName)
    {
        var imagePath = Path.Combine(folderName, fileName);

        return client.Storage.From(this.configuration["SupabaseStorage"] ?? "temp").GetPublicUrl(imagePath);
    }

    public async Task<string> UploadFileAsync(string folderName, string fileName, byte[] data)
    {
        var bucket = this.configuration["SupabaseStorage"] ?? "temp";
        var imagePath = Path.Combine(folderName, fileName);

        await client.Storage.From(bucket)
            .Upload(data, imagePath, new Supabase.Storage.FileOptions
            {
                Upsert = true
            });

        return client.Storage.From(bucket).GetPublicUrl(imagePath);
    }
}
