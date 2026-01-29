namespace DemoApi.Application.Models.Auth;

public class TokenResponse : ResponseViewModel
{
    public new TokenViewModel? Data { get; set; }
}