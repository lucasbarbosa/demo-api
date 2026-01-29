namespace DemoApi.Application.Models.Auth;

public class TokenViewModel : BaseViewModel
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public required string Created { get; set; }
    public required string Expires { get; set; }
}