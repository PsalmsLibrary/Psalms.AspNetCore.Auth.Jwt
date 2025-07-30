using Psalms.AspNetCore.Auth.Jwt.Models;
using Psalms.AspNetCore.Auth.Jwt.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken;

internal class EFRefreshTokenRepository(IPsalmsRefreshTokenEFContext context) : IPsalmsRefreshTokenRepository
{
    public async Task DeleteRefreshTokenAsync(string refreshToken)
    {
        context.Refreshes.Remove(await context.Refreshes.FirstAsync());
        await context.ConfirmChangesAsync();
    }

    public async Task<bool> RefreshTokenExistAsync(string refreshToken)
        => await context.Refreshes.AnyAsync(x => x.RefreshToken == refreshToken);

    public async Task SaveRefreshTokenAsync(RefreshTokenModel model)
    {
        await context.Refreshes.AddAsync(model);
        await context.ConfirmChangesAsync();
    }
}
