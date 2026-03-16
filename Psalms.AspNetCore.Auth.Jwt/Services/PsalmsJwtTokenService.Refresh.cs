using Microsoft.AspNetCore.Identity;
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
        var bytes = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);

        var token = Convert.ToBase64String(bytes);

        var hash = _refreshTokenHasher.Value.HashPassword(new(), token);

        RefreshTokenModel model = new()
        {
            Id = Guid.NewGuid(),
            RefreshToken = hash,        
        };

        if (_configuration["JWT:Expires"] is string expiresValue && int.TryParse(expiresValue, out int hours))
            model.Expires = DateTime.UtcNow.AddHours(hours);

        if (_refreshTokenRepository is not null)
            await _refreshTokenRepository.SaveRefreshTokenAsync(model);

        model.RefreshToken = token;

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
    
        var refreshToken = await _refreshTokenRepository!.GetByIdAsync(auth.RefreshTokenModel!.Id)
            ?? throw new ArgumentNullException("Refresh token is expired.");

        var result = _refreshTokenHasher.Value.VerifyHashedPassword(refreshToken, refreshToken.RefreshToken!, auth.RefreshTokenModel.RefreshToken!);

        if (result == PasswordVerificationResult.Failed) throw new Exception("Invalid refresh token.");

        await _refreshTokenRepository.DeleteRefreshTokenAsync(auth.RefreshTokenModel.Id);

        return await GetAuthResponseAsync(claims.Claims);
    }
}