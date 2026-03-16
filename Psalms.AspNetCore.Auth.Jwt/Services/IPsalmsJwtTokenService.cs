using Psalms.AspNetCore.Auth.Jwt.Models;
using System.Security.Claims;

namespace Psalms.AspNetCore.Auth.Jwt.Services;

public interface IPsalmsJwtTokenService
{
    /// <summary>
    /// Generates a JWT access token asynchronously based on the provided claims.
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    Task<string> GenerateAccessTokenAsync(IEnumerable<Claim>? claims);
    /// <summary>
    /// Asynchronously generates a new refresh token for authentication scenarios.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="RefreshTokenModel"/>
    /// representing the newly generated refresh token.</returns>
    Task<RefreshTokenModel> GenerateRefreshTokenAsync();
    /// <summary>
    /// Asynchronously generates an authentication response based on the provided claims.
    /// </summary>
    /// <param name="claims">A collection of claims to include in the authentication process. Can be null to indicate no claims.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an AuthResponse object with the
    /// authentication result.</returns>
    Task<AuthResponse> GetAuthResponseAsync(IEnumerable<Claim>? claims);
    /// <summary>
    /// Asynchronously refreshes the authentication token using the provided authentication response.
    /// </summary>
    /// <param name="auth">The current authentication response containing the access and refresh tokens to be used for obtaining a new
    /// token. Cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a new authentication response with
    /// refreshed tokens.</returns>
    Task<AuthResponse> RefreshTokenAsync(AuthResponse auth);
}
