namespace DH.Domain.Adapters.Email;

public enum EmailType
{
    RegistrationEmailConfirmation,
    EmployeePasswordCreation,
    ForgotPasswordReset,
}

public static class RegistrationEmailTemplateKeys
{
    public const string CallbackUrl = "CallbackUrl";
    public const string ClubName = "ClubName";
}

public static class EmployeePasswordCreation
{
    public const string ClubName = "ClubName";
    public const string CreatePasswordUrl = "CreatePasswordUrl";
}

public static class ForgotPasswordResetKeys
{
    public const string CallbackUrl = "CallbackUrl";
    public const string ClubName = "ClubName";
}