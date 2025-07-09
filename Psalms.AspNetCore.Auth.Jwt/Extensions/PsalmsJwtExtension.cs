using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Psalms.AspNetCore.Auth.Jwt;

public static class PsalmsJwtExtension
{
    public static IServiceCollection AddPsalmsJwtAuthentication(this IServiceCollection service, IConfiguration configuration)
    {
        var key = configuration["JWT:Key"] ?? throw new Exception("Key not found");

        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = !string.IsNullOrEmpty(configuration["JWT:Issuer"]),
                    ValidateAudience         = !string.IsNullOrEmpty(configuration["JWT:Audience"]),
                    ValidateLifetime         = !string.IsNullOrEmpty(configuration["JWT:Expires"]),
                    ValidateIssuerSigningKey = !string.IsNullOrEmpty(key),
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        return service;
    }
}