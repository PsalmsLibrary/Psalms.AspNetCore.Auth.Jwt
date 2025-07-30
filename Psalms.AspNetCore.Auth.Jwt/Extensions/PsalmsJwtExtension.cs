using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Psalms.Auth.Jwt;

namespace Psalms.AspNetCore.Auth.Jwt;

public static class PsalmsJwtExtension
{
    public static IServiceCollection AddPsalmsJwtAuthentication(this IServiceCollection service, IConfiguration configuration)
    {     
        service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                options.TokenValidationParameters = PsalmsJwtTokenService.GetValidationParameters(configuration));
   
        return service;
    }
}