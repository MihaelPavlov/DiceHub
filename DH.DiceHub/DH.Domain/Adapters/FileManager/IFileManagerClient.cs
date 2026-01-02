namespace DH.Domain.Adapters.FileManager;

public interface IFileManagerClient
{
    /// <summary>
    /// Uploads a file and returns a publicly accessible URL.
    /// </summary>
    /// <param name="fileName">The name of the file to save.</param>
    /// <param name="data">The file content as byte array.</param>
    /// <returns>URL to access the uploaded file</returns>
    Task<string> UploadFileAsync(string folderName, string fileName, byte[] data);

    string GetPublicUrl(string folderName, string fileName);
}
