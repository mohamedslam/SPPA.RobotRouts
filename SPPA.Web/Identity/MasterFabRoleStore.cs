// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using SPPA.Database;
using SPPA.Domain.Entities.Users;
using SPPA.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace SPPA.Web.Identity;

/// <summary>
/// Creates a new instance of a persistence store for roles.
/// </summary>
public class SPPARoleStore : IRoleStore<UserRole>
{
    /// <summary>
    /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
    /// </summary>
    private readonly IdentityErrorDescriber _errorDescriber;

    private readonly ApplicationDbContext _dbContext;

    private bool _disposed;

    /// <summary>
    /// Creates a new instance of the store.
    /// </summary>
    public SPPARoleStore(
        ApplicationDbContext dbContext
        )
    {
        _errorDescriber = new IdentityErrorDescriber();
        _dbContext = dbContext;
    }

    /// <summary>
    /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
    /// </summary>
    /// <value>
    /// True if changes should be automatically persisted, otherwise false.
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>Saves the current store.</summary>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    private async Task SaveChanges(CancellationToken cancellationToken)
    {
        if (AutoSaveChanges)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Creates a new role in a store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to create in the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public async Task<IdentityResult> CreateAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Create role not supported");
    }

    /// <summary>
    /// Updates a role in a store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to update in the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public async Task<IdentityResult> UpdateAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Update role not supported");
    }

    /// <summary>
    /// Deletes a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role to delete from the store.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that represents the <see cref="IdentityResult"/> of the asynchronous query.</returns>
    public async Task<IdentityResult> DeleteAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Delete role not supported");
    }

    /// <summary>
    /// Gets the ID for a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose ID should be returned.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the ID of the role.</returns>
    public virtual Task<string> GetRoleIdAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
        return Task.FromResult(role.Id.ToString());
    }

    /// <summary>
    /// Gets the name of a role from the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be returned.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
    public virtual Task<string> GetRoleNameAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
        return Task.FromResult(role.RoleType.ToString());
    }

    /// <summary>
    /// Sets the name of a role in the store as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose name should be set.</param>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task SetRoleNameAsync(UserRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Change role not supported");
    }

    /// <summary>
    /// Finds the role who has the specified ID as an asynchronous operation.
    /// </summary>
    /// <param name="id">The role ID to look for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
    public async Task<UserRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();

        if (!Guid.TryParse(roleId, out var id))
        {
            throw new MfBadRequestException("Incorrect UUID. Role id: " + roleId);
        }

        var role = await _dbContext.UserRoles.FindAsync(new object[] { id }, cancellationToken);
        if (role == null)
        {
            throw new MfNotFoundException("Role not found. User id: " + roleId);
        }
        return role;
    }

    /// <summary>
    /// Finds the role who has the specified normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="normalizedName">The normalized role name to look for.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that result of the look up.</returns>
    public async Task<UserRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Find by role name not supported");
    }

    /// <summary>
    /// Get a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be retrieved.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>A <see cref="Task{TResult}"/> that contains the name of the role.</returns>
    public virtual Task<string> GetNormalizedRoleNameAsync(UserRole role, CancellationToken cancellationToken = default(CancellationToken))
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        if (role == null)
        {
            throw new ArgumentNullException(nameof(role));
        }
        return Task.FromResult(role.RoleType.ToString().ToLowerInvariant());
    }

    /// <summary>
    /// Set a role's normalized name as an asynchronous operation.
    /// </summary>
    /// <param name="role">The role whose normalized name should be set.</param>
    /// <param name="normalizedName">The normalized name to set</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task SetNormalizedRoleNameAsync(UserRole role, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotSupportedException("Change role not supported");
    }

    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <summary>
    /// Dispose the stores
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
    }

    ///// <summary>
    ///// A navigation property for the roles the store contains.
    ///// </summary>
    //public IQueryable<UserRole> Roles
    //{
    //    get { return _dbContext.UserRoles.AsQueryable(); }
    //}

}
