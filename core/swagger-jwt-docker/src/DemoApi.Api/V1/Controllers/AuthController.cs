using Asp.Versioning;
using DemoApi.Api.Controllers;
using DemoApi.Api.Extensions;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Auth;
using DemoApi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace DemoApi.Api.V1.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class AuthController(INotificatorHandler notificator, IOptions<AuthorizationSettings> authorizationSettings) : MainApiController(notificator)
    {
        #region Properties

        private readonly AuthorizationSettings _authorizationSettings = authorizationSettings.Value;

        #endregion
        
        #region Public Methods

        [AllowAnonymous]
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseViewModel), StatusCodes.Status401Unauthorized)]
        public IActionResult GenerateToken([FromHeader(Name = "X-Security-Key")] string securityKey)
        {
            if (string.IsNullOrWhiteSpace(securityKey))
            {
                AddError("Security key is required. Please provide X-Security-Key header.");
                return CustomResponse(HttpStatusCode.Unauthorized, null);
            }

            if (securityKey != _authorizationSettings.SecurityKey)
            {
                AddError("Invalid security key. Authentication failed.");
                return CustomResponse(HttpStatusCode.Unauthorized, null);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_authorizationSettings.SecurityKey);
            var created = DateTime.UtcNow;
            var expires = created.AddMinutes(_authorizationSettings.ExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _authorizationSettings.Sender,
                Audience = _authorizationSettings.ValidOn,
                NotBefore = created,
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var encodedToken = tokenHandler.WriteToken(token);

            var tokenResponse = new TokenViewModel
            {
                AccessToken = encodedToken,
                TokenType = "Bearer",
                ExpiresIn = _authorizationSettings.ExpirationMinutes * 60,
                Created = created.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Expires = expires.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return CustomResponse(tokenResponse);
        }

        #endregion
    }
}