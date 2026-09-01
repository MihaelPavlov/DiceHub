namespace DH.Domain.Adapters.Authentication.Helpers;

public static class PasswordGenerator
{
    public static string GenerateRandomPassword()
    {
        var passwordLength = 12;
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+[]{};:<>?/";

        return new string(Enumerable.Range(0, passwordLength)
            .Select(_ => chars[new Random().Next(chars.Length)])
            .ToArray());
    }
}
