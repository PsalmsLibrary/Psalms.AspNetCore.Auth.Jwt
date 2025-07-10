using Microsoft.Extensions.Configuration;
using Psalms.Auth.Jwt;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Psalms.AspNetCore.Auth.Jwt.Tests.Services;

public class PsalmsJwtTokenServiceTests
{
    private IConfiguration BuildConfiguration(Dictionary<string, string?>? overrides = null)
    {
        var settings = new Dictionary<string, string?>
        {
            { "JWT:Key", "supersecretkey1234jfnsdfjgnb4y34y567890" },
            { "JWT:Issuer", "TestIssuer" },
            { "JWT:Audience", "TestAudience" },
            { "JWT:Expires", "1" }
        };

        if (overrides != null)
        {
            foreach (var kv in overrides)
                settings[kv.Key] = kv.Value;
        }

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public void GenerateToken_WithValidClaims_ReturnsValidJwt()
    {
        // Arrange
        var configuration = BuildConfiguration();
        var service = new PsalmsJwtTokenService(configuration);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "TestUser")
        };

        // Act
        string token = service.GenerateToken(claims);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("TestIssuer", jwtToken.Issuer);
        Assert.Equal("TestAudience", jwtToken.Audiences.First());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == "TestUser");
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "123");
    }

    [Fact]
    public void GenerateToken_WithNullKey_ThrowsException()
    {
        // Arrange
        var config = BuildConfiguration(new Dictionary<string, string?> { { "JWT:Key", null } });

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => new PsalmsJwtTokenService(config).GenerateToken(null));
        Assert.Equal("Key not found", ex.Message);
    }

    [Theory]
    [InlineData("-1")] 
    [InlineData("abc")] 
    [InlineData("99999")]
    public void GenerateToken_InvalidExpires_HandlesGracefully(string? expires)
    {
        // Arrange
        var config = BuildConfiguration(new Dictionary<string, string?> { { "JWT:Expires", expires } });
        var service = new PsalmsJwtTokenService(config);
        var claims = new[] { new Claim("user", "test") };

        // Act
        var token = service.GenerateToken(claims);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateToken_ReturnedTokenIsValidJwt()
    {
        // Arrange
        var config = BuildConfiguration();
        var service = new PsalmsJwtTokenService(config);
        var claims = new[] { new Claim("email", "user@test.com") };

        // Act
        var token = service.GenerateToken(claims);
        var handler = new JwtSecurityTokenHandler();

        // Assert
        Assert.True(handler.CanReadToken(token));
    }
}
