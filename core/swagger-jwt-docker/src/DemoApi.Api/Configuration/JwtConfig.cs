using System.Text;

using DemoApi.Api.Extensions;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace DemoApi.Api.Configuration;

public static class JwtConfig
{
    #region Public Methods

    public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var authorizationSection = configuration.GetSection("Authorization");
        var authorization = authorizationSection.Get<AuthorizationSettings>();

        if (authorization is null)
        {
            throw new InvalidOperationException(
                "JWT Authorization settings are missing in appsettings.json. " +
                "Please ensure 'Authorization' section is properly configured with SecurityKey, Sender, ValidOn, and ExpirationMinutes.");
        }

        if (string.IsNullOrWhiteSpace(authorization.SecurityKey))
        {
            throw new InvalidOperationException(
                "JWT SecurityKey is required and cannot be empty. " +
                "Please configure a valid SecurityKey in the Authorization section of appsettings.json.");
        }

        if (authorization.SecurityKey.Length < 32)
        {
            throw new InvalidOperationException(
                $"JWT SecurityKey must be at least 32 characters long for security reasons. " +
                $"Current length: {authorization.SecurityKey.Length}");
        }

        byte[] key = Encoding.UTF8.GetBytes(authorization.SecurityKey);

        services.Configure<AuthorizationSettings>(authorizationSection);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidAudience = authorization.ValidOn,
                ValidIssuer = authorization.Sender
            };
        });

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public static IApplicationBuilder UseJwtConfig(this IApplicationBuilder app)
    {
        app.UseAuthentication();

        app.UseAuthorization();

        return app;
    }

    #endregion
}