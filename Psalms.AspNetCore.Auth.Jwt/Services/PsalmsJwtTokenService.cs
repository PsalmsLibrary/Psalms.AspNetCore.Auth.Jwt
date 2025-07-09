using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Psalms.Auth.Jwt;

/// <summary>
/// Provides functionality to generate JWT tokens using configuration-driven values.
/// Part of the <c>Psalms</c> library family, this service offers a clean and reusable way
/// to issue signed tokens for authentication and authorization purposes.
/// </summary>
public class PsalmsJwtTokenService
{
    #region Attributes

    /// <summary>
    /// Lazily-initialized symmetric security key based on the configured <c>JWT:Key</c>.
    /// </summary>
    private readonly Lazy<SymmetricSecurityKey> _key;

    /// <summary>
    /// Lazily-initialized signing credentials using the symmetric key and HMAC-SHA256 algorithm.
    /// </summary>
    private readonly Lazy<SigningCredentials> _credentials;

    /// <summary>
    /// JWT token handler used to write and manage tokens.
    /// </summary>
    private readonly Lazy<JwtSecurityTokenHandler> _tokenHandler;

    /// <summary>
    /// Application configuration instance used to load values from <c>appsettings.json</c> or environment variables.
    /// </summary>
    private readonly IConfiguration _configuration;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="PsalmsJwtTokenService"/> class.
    /// </summary>
    /// <param name="configuration">
    /// Configuration instance that must contain the following keys:
    /// <list type="bullet">
    /// <item><description><c>JWT:Key</c> – the secret key used to sign tokens.</description></item>
    /// <item><description><c>JWT:Issuer</c> – the issuer (iss) of the token.</description></item>
    /// <item><description><c>JWT:Audience</c> – the audience (aud) of the token.</description></item>
    /// <item><description><c>JWT:Expires</c> – (optional) number of hours until the token expires.</description></item>
    /// </list>
    /// </param>
    public PsalmsJwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key           = new(() => GetSymmetricSecurityKeyLazy(configuration["JWT:Key"]));
        _credentials   = new(() => new SigningCredentials(_key.Value, SecurityAlgorithms.HmacSha256));
        _tokenHandler  = new(() => new JwtSecurityTokenHandler());
    }

    #endregion

    #region Methods
    /// <summary>
    /// Converts a raw key string into a <see cref="SymmetricSecurityKey"/> using UTF-8 encoding.
    /// </summary>
    /// <param name="key">The key string from configuration.</param>
    /// <returns>An instance of <see cref="SymmetricSecurityKey"/>.</returns>
    /// <exception cref="Exception">Thrown if the key is null or not found.</exception>
    private static SymmetricSecurityKey GetSymmetricSecurityKeyLazy(string? key)
        => new(Encoding.UTF8.GetBytes(key ?? throw new Exception("Key not found")));

    /// <summary>
    /// Generates a JWT token using the given claims and the configuration values.
    /// </summary>
    /// <param name="claims">An array of claims to include in the token payload.</param>
    /// <returns>A signed JWT token as a string.</returns>
    public string GenerateToken(Claim[] claims)
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

        return _tokenHandler.Value.WriteToken(token);
    }
    #endregion
}