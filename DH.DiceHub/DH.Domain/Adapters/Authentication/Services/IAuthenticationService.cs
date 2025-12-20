using DH.Domain.Adapters.Authentication.Models;

namespace DH.Domain.Adapters.Authentication.Services;

public interface IAuthenticationService
{
    Task<TokenResponseModel?> Login(LoginRequest form);
    Task<bool> Logout(string userId, string tenantId);
    Task<TokenResponseModel?> ConfirmEmail(string email, string token, CancellationToken cancellationToken);
    Task ResetPassword(ResetPasswordRequest request);
}
