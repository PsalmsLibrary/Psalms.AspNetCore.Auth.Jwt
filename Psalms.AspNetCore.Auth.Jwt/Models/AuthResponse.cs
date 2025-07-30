namespace Psalms.AspNetCore.Auth.Jwt.Models;

/// <summary>
/// Represents the response returned after a successful authentication or token refresh,
/// containing both access and refresh tokens.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Gets or sets the generated access token (JWT) used for authenticated requests.
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the associated refresh token model, used to obtain a new access token when the current one expires.
    /// </summary>
    public RefreshTokenModel? RefreshTokenModel { get; set; }
}