namespace DH.Adapter.Authentication.Services;

public interface IUserService
{
    Task<AuthenticatedResponse> Login(LoginForm form);
    Task Register(RegisterForm form);
}
