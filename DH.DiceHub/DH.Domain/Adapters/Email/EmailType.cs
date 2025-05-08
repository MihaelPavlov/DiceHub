namespace DH.Domain.Adapters.Email;

public enum EmailType
{
    RegistrationEmailConfirmation,
    EmployeePasswordGeneration,
}

public static class RegistrationEmailTemplateKeys
{
    public const string CallbackUrl = "CallbackUrl";
    public const string ClubName = "ClubName";
}