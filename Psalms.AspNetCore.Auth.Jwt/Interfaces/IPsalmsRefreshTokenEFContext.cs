using Microsoft.EntityFrameworkCore;
using Psalms.AspNetCore.Auth.Jwt.Models;

namespace Psalms.AspNetCore.Auth.Jwt.Interfaces;

/// <summary>
/// Represents a contract for the Entity Framework Core context used to manage refresh tokens.
/// This abstraction allows for decoupling of the JWT service from the underlying EF Core implementation,
/// enabling easier testing and swapping of persistence strategies.
/// </summary>
public interface IPsalmsRefreshTokenEFContext
{
    /// <summary>
    /// The set of <see cref="RefreshTokenModel"/> entities tracked by the context.
    /// Used to query, insert, update, or delete refresh tokens in the underlying database.
    /// </summary>
    DbSet<RefreshTokenModel> Refreshes { get; set; }

    /// <summary>
    /// Persists all changes made in the context to the database asynchronously.
    /// Should be called after modifying the <see cref="Refreshes"/> set.
    /// </summary>
    Task ConfirmChangesAsync();
}