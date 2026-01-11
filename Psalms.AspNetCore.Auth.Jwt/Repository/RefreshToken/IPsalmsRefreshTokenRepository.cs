using Psalms.AspNetCore.Auth.Jwt.Models;

namespace Psalms.AspNetCore.Auth.Jwt.Repository.RefreshToken;

/// <summary>
/// Interface for managing refresh tokens in a persistent storage.
/// </summary>
public interface IPsalmsRefreshTokenRepository
{
    /// <summary>
    /// Saves a new refresh token to the storage.
    /// </summary>
    /// <param name="model">The refresh token model to be saved.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SaveRefreshTokenAsync(RefreshTokenModel model);

    /// <summary>
    /// Deletes an existing refresh token from the storage.
    /// </summary>
    /// <param name="token">The refresh token to be deleted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteRefreshTokenAsync(Guid id);

    /// <summary>
    /// Checks if a given refresh token exists in the storage.
    /// </summary>
    /// <param name="token">The refresh token to be verified.</param>
    /// <returns>A task that returns true if the token exists, otherwise false.</returns>
    Task<bool> RefreshTokenExistAsync(Guid id);
    /// <summary>
    /// Asynchronously retrieves a refresh token by its unique identifier.  
    /// </summary>
    /// <param name="id">The unique identifier of the refresh token to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="RefreshTokenModel"/>
    /// if found; otherwise, <c>null</c>.</returns>
    Task<RefreshTokenModel> GetByIdAsync(Guid id);
}
