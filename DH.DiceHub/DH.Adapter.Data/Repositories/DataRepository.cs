﻿using DH.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DH.Adapter.Data.Repositories;

/// <summary>
/// A data repository implementation for performing CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class DataRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    readonly TenantDbContext tenantDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataRepository{TEntity}"/> class.
    /// </summary>
    /// <param name="tenantDbContext">The DbContext to be used for data operations.</param>
    public DataRepository(TenantDbContext tenantDbContext)
    {
        this.tenantDbContext = tenantDbContext;
    }

    /// <inheritdoc/>
    public async Task<List<TResult>> GetWithPropertiesAsync<TResult>(
     Expression<Func<TEntity, TResult>> selector,
     CancellationToken cancellationToken)
    {
        return await this.tenantDbContext.Set<TEntity>()
            .Select(selector)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetByAsync(Expression<Func<TEntity, bool>> selector, CancellationToken cancellationToken)
    {
        return await this.tenantDbContext.Set<TEntity>().FirstOrDefaultAsync(selector, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var result = await this.tenantDbContext.AddAsync(entity, cancellationToken);
        await this.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await this.tenantDbContext.AddRangeAsync(entities, cancellationToken);
        await this.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async void Remove(TEntity entity, CancellationToken cancellationToken)
    {
        this.tenantDbContext.Remove(entity);
        await this.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async void RemoveRange(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        this.tenantDbContext.RemoveRange(entities);
        await this.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async void Update(TEntity entity, CancellationToken cancellationToken)
    {
        this.tenantDbContext.Update(entity);
        await this.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously saves all changes made in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await this.tenantDbContext.SaveChangesAsync(cancellationToken);
    }
}
