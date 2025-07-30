using Microsoft.EntityFrameworkCore;
using Psalms.AspNetCore.Auth.Jwt.Models;

namespace Psalms.AspNetCore.Auth.Jwt.Interfaces;
    
public interface IPsalmsRefreshTokenEFContext
{
    DbSet<RefreshTokenModel> Refreshes { get; set; }
    Task ConfirmChangesAsync();
}