using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Psalms.AspNetCore.Auth.Jwt.Services;
using Psalms.Auth.Jwt;

namespace Psalms.AspNetCore.Auth.Jwt.Extensions;

public static class PsalmsJwtExtension
{
    public static IServiceCollection AddPsalmsJwtAuthentication(this IServiceCollection service, IConfiguration configuration)
    {     
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = PsalmsJwtTokenService.GetValidationParameters(configuration));

        service.AddScoped<IPsalmsJwtTokenService, PsalmsJwtTokenService>();
   
        return service;
    }
}