namespace DH.Domain.Adapters.Email;

public enum EmailType
{
    RegistrationEmailConfirmation,
    EmployeePasswordCreation,
    ForgotPasswordReset,
    PartnerInquiryRequest,
    OwnerPasswordCreation,
    TenantApplicationEmailVerification,
    TenantSetupInvitation,
    TenantOwnerCredentials,
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

public static class PartnerInquiryRequest
{
    public const string Name = "Name";
    public const string Email = "Email";
    public const string Message = "Message";
    public const string PhoneNumber= "PhoneNumber";
}

public static class OwnerPasswordCreation
{
    public const string ClubName = "ClubName";
    public const string CreatePasswordUrl = "CreatePasswordUrl";
}

public static class TenantApplicationEmailVerification
{
    public const string VerificationCode = "VerificationCode";
}

public static class TenantSetupInvitation
{
    public const string ClubName = "ClubName";
    public const string SetupUrl = "SetupUrl";
    public const string ExpiresInHours = "ExpiresInHours";
}

public static class TenantOwnerCredentials
{
    public const string ClubName = "ClubName";
    public const string LoginUrl = "LoginUrl";
    public const string Email = "Email";
    public const string Password = "Password";
}
