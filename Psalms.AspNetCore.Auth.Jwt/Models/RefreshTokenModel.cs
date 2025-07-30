using System.ComponentModel.DataAnnotations;

namespace Psalms.AspNetCore.Auth.Jwt.Models;

public class RefreshTokenModel
{
    [Key]
    public string? RefreshToken { get; set; } 
}