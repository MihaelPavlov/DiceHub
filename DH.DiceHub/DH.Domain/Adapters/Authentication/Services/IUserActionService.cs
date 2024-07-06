namespace DH.Domain.Adapters.Authentication.Services;

public interface IUserActionService
{
    bool IsActionAvailable(int actionKey);
}
