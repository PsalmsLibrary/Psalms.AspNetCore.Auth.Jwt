using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Psalms.AspNetCore.Auth.Jwt.Interfaces;
using Psalms.AspNetCore.Auth.Jwt.Models;
using Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken;
using System.IdentityModel.Tokens.Jwt;

namespace Psalms.Auth.Jwt;

/// <summary>
/// Provides functionality to generate JWT tokens using configuration-driven values.
/// Part of the <c>Psalms</c> library family, this service offers a clean and reusable way
/// to issue signed tokens for authentication and authorization purposes.
/// </summary>
public partial class PsalmsJwtTokenService
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
    /// Provides lazy initialization for the password hasher used with refresh token models.
    /// </summary>
    private readonly Lazy<IPasswordHasher<RefreshTokenModel>> _refreshTokenHasher;

    /// <summary>
    /// Application configuration instance used to load values from <c>appsettings.json</c> or environment variables.
    /// </summary>
    private readonly IConfiguration _configuration;
    private readonly IPsalmsRefreshTokenRepository? _refreshTokenRepository;

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
        _configuration      = configuration;
        _key                = new(() => GetSymmetricSecurityKeyLazy(configuration["JWT:Key"]));
        _credentials        = new(() => new SigningCredentials(_key.Value, SecurityAlgorithms.HmacSha256));
        _tokenHandler       = new(() => new JwtSecurityTokenHandler());
        _refreshTokenHasher = new(() => new PasswordHasher<RefreshTokenModel>());
    }
    public PsalmsJwtTokenService(IConfiguration configuration, IPsalmsRefreshTokenEFContext context) : this(configuration)
        => _refreshTokenRepository = new EFRefreshTokenRepository(context);
    public PsalmsJwtTokenService(IConfiguration configuration, IPsalmsRefreshTokenRepository repository) : this(configuration)
        => _refreshTokenRepository = repository;
    #endregion
}