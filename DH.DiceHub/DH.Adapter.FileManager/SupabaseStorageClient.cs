using DH.Domain.Adapters.FileManager;

namespace DH.Adapter.FileManager;

public class SupabaseStorageClient : IFileManagerClient
{
    readonly Supabase.Client client;
    public SupabaseStorageClient(Supabase.Client client)
    {
        this.client = client;
    }

    public string GetPublicUrl(string folderName, string fileName)
    {
        var imagePath = Path.Combine(folderName, fileName);

        return client.Storage.From("dicehub").GetPublicUrl(imagePath);
    }

    public async Task<string> UploadFileAsync(string folderName, string fileName, byte[] data)
    {
        var imagePath = Path.Combine(folderName, fileName);

        await client.Storage.From("dicehub")
            .Upload(data, imagePath, new Supabase.Storage.FileOptions
            {
                Upsert = true
            });

        return client.Storage.From("dicehub").GetPublicUrl(imagePath);
    }
}
