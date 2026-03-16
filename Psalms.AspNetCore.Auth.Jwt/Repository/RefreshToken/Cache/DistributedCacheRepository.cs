using Microsoft.Extensions.Caching.Distributed;
using Psalms.AspNetCore.Auth.Jwt.Models;

namespace Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken.Cache;

public class DistributedCacheRepository(IDistributedCache cache) : IPsalmsRefreshTokenRepository
{
    public async Task DeleteRefreshTokenAsync(Guid id) => await cache.RemoveAsync(id.ToString());

    public async Task<RefreshTokenModel?> GetByIdAsync(Guid id)
        => await cache.GetStringAsync(id.ToString()) is string value
            ? new RefreshTokenModel { Id = id, RefreshToken = value }
            : null;

    public async Task<bool> RefreshTokenExistAsync(Guid id)
        => await cache.GetStringAsync(id.ToString()).ContinueWith(task => task.Result is not null);

    public async Task SaveRefreshTokenAsync(RefreshTokenModel model)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = new DateTimeOffset(model.Expires)
        };

        await cache.SetStringAsync(
            $"refresh-token:{model.Id}",
            model.RefreshToken ?? throw new ArgumentNullException(nameof(model.RefreshToken)),
            options
        );
    }
}