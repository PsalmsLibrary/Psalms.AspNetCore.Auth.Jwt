using System.ComponentModel.DataAnnotations;

namespace Psalms.AspNetCore.Auth.Jwt.Models;

/// <summary>
/// Represents a refresh token used to obtain a new access token after expiration.
/// This model is typically stored in a database or cache for validation during token refresh.
/// </summary>
public class RefreshTokenModel
{
    [Key]
    public Guid Id { get; set; }
    public string? RefreshToken { get; set; }
}