using System.Linq.Expressions;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Base;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(filter).ToListAsync(cancellationToken);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        DbSet.Update(entity);
    }

    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        DbSet.UpdateRange(entities);
    }

    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        return filter is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(filter, cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(filter, cancellationToken);
    }
}