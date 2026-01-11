using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Psalms.AspNetCore.Auth.Jwt.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Psalms.Auth.Jwt;

public partial class PsalmsJwtTokenService
{
    /// <summary>
    /// Converts a raw key string into a <see cref="SymmetricSecurityKey"/> using UTF-8 encoding.
    /// </summary>
    /// <param name="key">The key string from configuration.</param>
    /// <returns>An instance of <see cref="SymmetricSecurityKey"/>.</returns>
    /// <exception cref="Exception">Thrown if the key is null or not found.</exception>
    private static SymmetricSecurityKey GetSymmetricSecurityKeyLazy(string? key)
        => new(Encoding.UTF8.GetBytes(key ?? throw new Exception("Key not found")));

    /// <summary>
    /// Extracts the <see cref="ClaimsPrincipal"/> from an expired JWT access token.
    /// This method validates the token signature but ignores token expiration to retrieve claims.
    /// </summary>
    /// <param name="token">The expired JWT access token.</param>
    /// <returns>A task containing the <see cref="ClaimsPrincipal"/> extracted from the token.</returns>
    /// <exception cref="Exception">Thrown when the token is invalid or uses an unexpected algorithm.</exception>
    private Task<ClaimsPrincipal> GetClaimsPrincipalInExpiredTokenAsync(string token)
    {
        var tokenParameters = GetValidationParameters(_configuration);

        var claims = _tokenHandler.Value.ValidateToken(token, tokenParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurity ||
            !jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new Exception("Invalid token.");

        return Task.FromResult(claims);
    }

    /// <summary>
    /// Validates the given authentication response for the presence and existence of tokens.
    /// Throws exceptions if any validation fails.
    /// </summary>
    /// <param name="auth">The authentication response to validate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NullReferenceException">Thrown when tokens are null.</exception>
    /// <exception cref="Exception">Thrown when repository is not found or refresh token does not exist.</exception>
    private async Task ValidateAuthResponseAsync(AuthResponse auth)
    {
        if (auth.AccessToken is null || auth.RefreshTokenModel is null)
            throw new NullReferenceException("Auth response is null");

        if (auth.RefreshTokenModel.RefreshToken is null) throw new NullReferenceException("Refresh token is null.");

        if (_refreshTokenRepository is null) throw new Exception("RefreshTokenRepository was not configured.");
    }

    /// <summary>
    /// Creates <see cref="TokenValidationParameters"/> for validating JWT tokens.
    /// Pulls settings from the configuration, including issuer, audience, and signing key.
    /// </summary>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    /// <returns>A configured <see cref="TokenValidationParameters"/> object.</returns>
    /// <exception cref="Exception">Thrown when the signing key is not found in configuration.</exception>
    internal static TokenValidationParameters GetValidationParameters(IConfiguration configuration)
    {
        var key = configuration["JWT:Key"] ?? throw new Exception("Key not found");

        return new TokenValidationParameters
        {
            ValidateIssuer           = !string.IsNullOrEmpty(configuration["JWT:Issuer"]),
            ValidateAudience         = !string.IsNullOrEmpty(configuration["JWT:Audience"]),
            ValidateLifetime         = !string.IsNullOrEmpty(configuration["JWT:Expires"]),
            ValidateIssuerSigningKey = !string.IsNullOrEmpty(key),
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    }

}