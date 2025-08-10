using BiteDanceAPI.Domain.Common;

namespace BiteDanceAPI.Application.Common.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> FindOrNotFoundExceptionAsync<T, TKey>(
        this DbSet<T> query,
        TKey key,
        CancellationToken cancellationToken
    )
        where T : class
        where TKey : struct
    {
        var entity = await query.FindAsync([key], cancellationToken: cancellationToken);
        Guard.Against.NotFound(key, entity);

        return entity;
    }

    public static async Task<T> FindOrNotFoundExceptionAsync<T>(
        this IQueryable<T> query,
        int key,
        CancellationToken cancellationToken
    )
        where T : BaseEntity
    {
        var entity = await query.FirstOrDefaultAsync(
            e => e.Id == key,
            cancellationToken: cancellationToken
        );
        Guard.Against.NotFound(key, entity);

        return entity;
    }
}
