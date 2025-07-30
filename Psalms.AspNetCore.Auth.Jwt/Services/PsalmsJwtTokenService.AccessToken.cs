using Psalms.AspNetCore.Auth.Jwt.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Psalms.Auth.Jwt;

public partial class PsalmsJwtTokenService
{
    /// <summary>
    /// Generates a JWT token using the given claims and the configuration values.
    /// </summary>
    /// <param name="claims">An array of claims to include in the token payload.</param>
    /// <returns>A signed JWT token as a string.</returns>
    public Task<string> GenerateAccessTokenAsync(IEnumerable<Claim>? claims)
    {
        DateTime? expires = null;

        if (_configuration["JWT:Expires"] is string expiresValue && int.TryParse(expiresValue, out int hours))
            expires = DateTime.UtcNow.AddHours(hours);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: _credentials.Value
        );

        return Task.FromResult(_tokenHandler.Value.WriteToken(token));
    }

    /// <summary>
    /// Generates an <see cref="AuthResponse"/> containing an access token and a refresh token.
    /// </summary>
    /// <param name="claims">The claims to include in the generated access token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="AuthResponse"/> 
    /// with the generated access token and refresh token model.
    /// </returns>
    public async Task<AuthResponse> GetAuthResponseAsync(IEnumerable<Claim>? claims)
    {
        return new()
        {
            AccessToken       = await GenerateAccessTokenAsync(claims),
            RefreshTokenModel = await GenerateRefreshTokenAsync()
        };
    }
}