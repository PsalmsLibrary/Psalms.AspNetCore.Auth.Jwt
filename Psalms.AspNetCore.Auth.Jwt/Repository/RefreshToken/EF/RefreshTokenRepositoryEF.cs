using Psalms.AspNetCore.Auth.Jwt.Models;
using Psalms.AspNetCore.Auth.Jwt.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken;

internal class EFRefreshTokenRepository(IPsalmsRefreshTokenEFContext context) : IPsalmsRefreshTokenRepository
{
    public async Task DeleteRefreshTokenAsync(Guid id)
    {
        context.Refreshes.Remove(await context.Refreshes.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new Exception("Refresh token not found to delete."));
        await context.ConfirmChangesAsync();
    }

    public async Task<bool> RefreshTokenExistAsync(Guid id)
        => await context.Refreshes.AnyAsync(x => x.Id == id);

    public async Task SaveRefreshTokenAsync(RefreshTokenModel model)
    {
        await context.Refreshes.AddAsync(model);
        await context.ConfirmChangesAsync();
    }

    public async Task<RefreshTokenModel> GetByIdAsync(Guid id) => await context.Refreshes.FirstOrDefaultAsync(x => x.Id == id)
            ?? throw new Exception("Refresh token not found.");
}