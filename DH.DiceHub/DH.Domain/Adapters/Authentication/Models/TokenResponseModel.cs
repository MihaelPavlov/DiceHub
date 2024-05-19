namespace DH.Domain.Adapters.Authentication.Models;

public class TokenResponseModel
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
