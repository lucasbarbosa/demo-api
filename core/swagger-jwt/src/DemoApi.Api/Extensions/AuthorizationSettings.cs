namespace DemoApi.Api.Extensions;

public class AuthorizationSettings
{
    public required string SecurityKey { get; set; }
    public int ExpirationMinutes { get; set; }
    public required string Sender { get; set; }
    public required string ValidOn { get; set; }
}