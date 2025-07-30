using Microsoft.Extensions.Caching.Memory;
using Psalms.AspNetCore.Auth.Jwt.Models;

namespace Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken.Memory;

internal class RefreshTokenRepositoryInMemory(IMemoryCache cache) : IPsalmsRefreshTokenRepository
{
    public Task DeleteRefreshTokenAsync(string token)
    {
        cache.Remove(token);
        return Task.CompletedTask;
    }   

    public Task<bool> RefreshTokenExistAsync(string token)
    {
        return Task.FromResult(cache.Get(token) is not null);
    }

    public Task SaveRefreshTokenAsync(RefreshTokenModel model)
    {
        cache.Set(model.RefreshToken!, model);
        return Task.CompletedTask;
    }
}
