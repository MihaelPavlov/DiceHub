namespace DH.Domain.Adapters.Email;

public enum EmailType
{
    RegistrationEmailConfirmation,
    EmployeePasswordGeneration,
    ForgotPasswordReset,
}

public static class RegistrationEmailTemplateKeys
{
    public const string CallbackUrl = "CallbackUrl";
    public const string ClubName = "ClubName";
}

public static class ForgotPasswordResetKeys
{
    public const string CallbackUrl = "CallbackUrl";
    public const string ClubName = "ClubName";
}