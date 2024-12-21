namespace DH.Messaging.HttpClient.Helpers;

public static class Sanitizer
{
    public static string SanitizeFileName(string fileName)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
            fileName = fileName.Replace(invalidChar, '_');

        return fileName;
    }
    public static string? SanitizeCRLF(string? value)
    {
        if (value is null)
            return value;

        return value.Replace("\r", "").Replace("\n", "");
    }
}

