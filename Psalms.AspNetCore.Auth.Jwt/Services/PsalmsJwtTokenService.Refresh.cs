using Psalms.AspNetCore.Auth.Jwt.Models;
using System.Security.Cryptography;

namespace Psalms.Auth.Jwt;

/// <summary>
/// Service responsible for generating and validating JWT refresh tokens,
/// including refreshing authentication tokens.
/// </summary>
public partial class PsalmsJwtTokenService
{
    /// <summary>
    /// Generates a new secure refresh token asynchronously and saves it to the repository if available.
    /// </summary>
    /// <returns>A <see cref="RefreshTokenModel"/> containing the generated refresh token.</returns>
    public async Task<RefreshTokenModel> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = Convert.ToBase64String(randomNumber);

        RefreshTokenModel model = new() { RefreshToken = refreshToken };

        if (_refreshTokenRepository is not null)
            await _refreshTokenRepository.SaveRefreshTokenAsync(model);

        return model;
    }

    /// <summary>
    /// Refreshes the authentication tokens based on the provided authentication response.
    /// Validates the refresh token, deletes the old token, and generates new tokens.
    /// </summary>
    /// <param name="auth">The current authentication response containing access and refresh tokens.</param>
    /// <returns>A new <see cref="AuthResponse"/> with updated access and refresh tokens.</returns>
    public async Task<AuthResponse> RefreshTokenAsync(AuthResponse auth)
    {
        await ValidateAuthResponseAsync(auth);

        var claims = await GetClaimsPrincipalInExpiredTokenAsync(auth.AccessToken!);

        await _refreshTokenRepository!.DeleteRefreshTokenAsync(auth.RefreshTokenModel!.RefreshToken!);

        return await GetAuthResponseAsync(claims.Claims);
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

        if (!await _refreshTokenRepository.RefreshTokenExistAsync(auth.RefreshTokenModel.RefreshToken))
            throw new Exception("refresh token does no exist");
    }
}
